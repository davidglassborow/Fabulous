﻿// Copyright 2018 Elmish.XamarinForms contributors. See LICENSE.md for license.
namespace AllControls

open System
open Elmish.XamarinForms
open Elmish.XamarinForms.DynamicViews
open Xamarin.Forms

type RootPageKind = 
    | Choice of bool
    | Tabbed1 
    | Tabbed2 
    | Tabbed3 
    | Navigation 
    | Carousel 
    | MasterDetail
    | InfiniteScrollList

type Model = 
  { RootPageKind: RootPageKind
    Count : int
    CountForSlider : int
    CountForActivityIndicator : int
    StepForSlider : int 
    StartDate : System.DateTime
    EndDate : System.DateTime
    EditorText : string
    EntryText : string
    Placeholder : string
    Password : string
    NumTaps : int 
    NumTaps2 : int 
    PickedColorIndex: int
    GridSize: int
    NewGridSize: double // used during pinch
    GridPortal: int * int 
    // For MasterDetailPage demo
    IsMasterPresented: bool 
    DetailPage: string
    // For NavigationPage demo
    PageStack: string option list
    // For InfiniteScroll page demo. It's not really an "infinite" scroll, just an unbounded set of data whose growth is prompted by the need formore of it in the UI
    InfiniteScrollMaxRequested: int
    SearchTerm: string
    }

type Msg = 
    | Increment 
    | Decrement 
    | Reset
    | IncrementForSlider
    | DecrementForSlider
    | IncrementForActivityIndicator
    | DecrementForActivityIndicator
    | SliderValueChanged of int
    | TextChanged of string * string
    | EditorEditCompleted of string
    | EntryEditCompleted of string
    | PasswordEntryEditCompleted of string
    | PlaceholderEntryEditCompleted of string
    | GridEditCompleted of int * int
    | StartDateSelected of DateTime 
    | EndDateSelected of DateTime 
    | PickerItemChanged of int
    | ListViewSelectedItemChanged of int option
    | ListViewGroupedSelectedItemChanged of (int * int) option
    | FrameTapped 
    | FrameTapped2 
    | UpdateNewGridSize of double * GestureStatus
    | UpdateGridPortal of int * int
    // For NavigationPage demo
    | GoHomePage
    | PopPage 
    | PagePopped 
    | ReplacePage of string
    | PushPage of string
    | SetRootPageKind of RootPageKind
    // For MasterDetail page demo
    | IsMasterPresentedChanged of bool
    | SetDetailPage of string
    // For InfiniteScroll page demo. It's not really an "infinite" scroll, just a growing set of "data"
    | SetInfiniteScrollMaxIndex of int
    | ExecuteSearch of string

[<AutoOpen>]
module MyExtension = 
    /// Test the extension API be making a 2nd wrapper for "Label":
    let TestLabelTextAttribKey = AttributeKey<_> "TestLabel_Text"
    let TestLabelFontFamilyAttribKey = AttributeKey<_> "TestLabel_FontFamily"

    type View with 

        static member TestLabel(?text: string, ?fontFamily: string, ?backgroundColor, ?rotation) = 

            // Get the attributes for the base element. The number is the the expected number of attributes.
            // You can add additional base element attributes here if you like
            let attribCount = 0
            let attribCount = match text with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fontFamily with Some _ -> attribCount + 1 | None -> attribCount
            let attribs = View.BuildView(attribCount, ?backgroundColor = backgroundColor, ?rotation = rotation) 

            // Add our own attributes. They must have unique names.
            match text with None -> () | Some v -> attribs.Add(TestLabelTextAttribKey, v) 
            match fontFamily with None -> () | Some v -> attribs.Add(TestLabelFontFamilyAttribKey, v) 

            // The creation method
            let create () = new Xamarin.Forms.Label()

            // The incremental update method
            let update (prevOpt: ViewElement voption) (source: ViewElement) (target: Xamarin.Forms.Label) = 
                View.UpdateView(prevOpt, source, target)
                source.UpdatePrimitive(prevOpt, target, TestLabelTextAttribKey, (fun target v -> target.Text <- v))
                source.UpdatePrimitive(prevOpt, target, TestLabelFontFamilyAttribKey, (fun target v -> target.FontFamily <- v))

            ViewElement.Create<Xamarin.Forms.Label>(create, update, attribs)

    // Test some adhoc functional abstractions
    type View with 
        static member ScrollingContentPage(title, children) =
            View.ContentPage(title=title, content=View.ScrollView(View.StackLayout(padding=20.0, children=children) ), useSafeArea=true)

        static member NonScrollingContentPage(title, children, ?gestureRecognizers) =
            View.ContentPage(title=title, content=View.StackLayout(padding=20.0, children=children, ?gestureRecognizers=gestureRecognizers), useSafeArea=true)


module App = 
    let init () = 
        { RootPageKind = Choice false
          Count = 0
          CountForSlider = 0
          CountForActivityIndicator = 0
          StepForSlider = 3
          PickedColorIndex = 0
          EditorText = "hic hac hoc"
          Placeholder = "cogito ergo sum"
          Password = "in omnibus errant"
          EntryText = "quod erat demonstrandum"
          GridSize = 6
          NewGridSize = 6.0
          GridPortal=(0, 0)
          StartDate=System.DateTime.Today
          EndDate=System.DateTime.Today.AddDays(1.0)
          IsMasterPresented=false
          NumTaps=0
          NumTaps2=0
          PageStack=[ Some "Home" ]
          DetailPage="A"
          InfiniteScrollMaxRequested = 10 
          SearchTerm = "nothing!"}

    let update msg model =
        match msg with
        | Increment -> { model with Count = model.Count + 1 }
        | Decrement -> { model with Count = model.Count - 1}
        | IncrementForSlider -> { model with CountForSlider = model.CountForSlider + model.StepForSlider }
        | DecrementForSlider -> { model with CountForSlider = model.CountForSlider - model.StepForSlider }
        | IncrementForActivityIndicator -> { model with CountForActivityIndicator = model.CountForActivityIndicator + 1 }
        | DecrementForActivityIndicator -> { model with CountForActivityIndicator = model.CountForActivityIndicator - 1 }
        | Reset -> init ()
        | SliderValueChanged n -> { model with StepForSlider = n }
        | TextChanged (oldValue, newValue) -> model
        | EditorEditCompleted t -> { model with EditorText = t }
        | EntryEditCompleted t -> { model with EntryText = t }
        | PasswordEntryEditCompleted t -> { model with Password = t }
        | PlaceholderEntryEditCompleted t -> { model with Placeholder = t }
        | StartDateSelected d -> { model with StartDate = d; EndDate = d + (model.EndDate - model.StartDate) }
        | EndDateSelected d -> { model with EndDate = d }
        | GridEditCompleted (i, j) -> model
        | ListViewSelectedItemChanged item -> model
        | ListViewGroupedSelectedItemChanged item -> model
        | PickerItemChanged i -> { model with PickedColorIndex = i }
        | FrameTapped -> { model with NumTaps= model.NumTaps + 1 }
        | FrameTapped2 -> { model with NumTaps2= model.NumTaps2 + 1 }
        | UpdateNewGridSize (n, status) -> 
            match status with 
            | GestureStatus.Running -> { model with NewGridSize = model.NewGridSize * n}
            | GestureStatus.Completed -> let sz = int (model.NewGridSize + 0.5) in { model with GridSize = sz; NewGridSize = float sz }
            | GestureStatus.Canceled -> { model with NewGridSize = double model.GridSize }
            | _ -> model
        | UpdateGridPortal (x, y) -> { model with GridPortal = (x, y) }
        // For NavigationPage
        | GoHomePage -> { model with PageStack = [ Some "Home" ] }
        | PagePopped -> 
            if model.PageStack |> List.exists Option.isNone then 
               { model with PageStack = model.PageStack |> List.filter Option.isSome }
            else
               { model with PageStack = (match model.PageStack with [] -> model.PageStack | _ :: t -> t) }
        | PopPage -> 
               { model with PageStack = (match model.PageStack with [] -> model.PageStack | _ :: t -> None :: t) }
        | PushPage page -> 
            { model with PageStack = Some page :: model.PageStack}
        | ReplacePage page -> 
            { model with PageStack = (match model.PageStack with [] -> Some page :: model.PageStack | _ :: t -> Some page :: t) }
        // For MasterDetail
        | IsMasterPresentedChanged b -> { model with IsMasterPresented = b }
        | SetDetailPage s -> { model with DetailPage = s ; IsMasterPresented=false}
        | SetInfiniteScrollMaxIndex n -> if n >= max n model.InfiniteScrollMaxRequested then { model with InfiniteScrollMaxRequested = (n + 10)} else model
        // For selection page
        | SetRootPageKind kind -> { model with RootPageKind = kind }
        | ExecuteSearch search -> { model with SearchTerm = search }

    let pickerItems = 
        [| ("Aqua", Color.Aqua); ("Black", Color.Black);
           ("Blue", Color.Blue); ("Fucshia", Color.Fuchsia);
           ("Gray", Color.Gray); ("Green", Color.Green);
           ("Lime", Color.Lime); ("Maroon", Color.Maroon);
           ("Navy", Color.Navy); ("Olive", Color.Olive);
           ("Purple", Color.Purple); ("Red", Color.Red);
           ("Silver", Color.Silver); ("Teal", Color.Teal);
           ("White", Color.White); ("Yellow", Color.Yellow ) |]

    let view (model: Model) dispatch =

        match model.RootPageKind with 
        | Choice showAbout -> 
            View.NavigationPage(pages=
                [ yield 
                    View.ContentPage(title="Root Page", useSafeArea=true,
                        padding = new Thickness (10.0, 20.0, 10.0, 5.0), 
                        content= View.StackLayout(
                            children=[ 
                                 View.Button(text = "TabbedPage #1 (various controls)", command=(fun () -> dispatch (SetRootPageKind Tabbed1)))
                                 View.Button(text = "TabbedPage #2 (various controls)", command=(fun () -> dispatch (SetRootPageKind Tabbed2)))
                                 View.Button(text = "TabbedPage #3 (various controls)", command=(fun () -> dispatch (SetRootPageKind Tabbed3)))
                                 View.Button(text = "CarouselPage (various controls)", command=(fun () -> dispatch (SetRootPageKind Carousel)))
                                 View.Button(text = "NavigationPage with push/pop", command=(fun () -> dispatch (SetRootPageKind Navigation)))
                                 View.Button(text = "MasterDetail Page", command=(fun () -> dispatch (SetRootPageKind MasterDetail)))
                                 View.Button(text = "Infinite scrolling ListView", command=(fun () -> dispatch (SetRootPageKind InfiniteScrollList)))
                            ]))
                     .ToolbarItems([View.ToolbarItem(text="About", command=(fun () -> dispatch (SetRootPageKind (Choice true))))] )
                  if showAbout then 
                    yield 
                        View.ContentPage(title="About", useSafeArea=true, 
                            padding = new Thickness (10.0, 20.0, 10.0, 5.0), 
                            content= View.StackLayout(
                               children=[ 
                                   View.TestLabel(text = "Elmish.XamarinForms, version " + string (typeof<ViewElement>.Assembly.GetName().Version))
                                   View.Button(text = "Continue", command=(fun () -> dispatch (SetRootPageKind (Choice false)) ))
                               ]))
                ])

        | Carousel -> 
           View.CarouselPage(useSafeArea=true, children=
             [ dependsOn model.Count (fun model count -> 
                   View.ScrollingContentPage("Button", 
                       [ View.Label(text="Label:")
                         View.Label(text= sprintf "%d" count, horizontalOptions=LayoutOptions.CenterAndExpand)
                 
                         View.Label(text="Button:")
                         View.Button(text="Increment", command=(fun () -> dispatch Increment), horizontalOptions=LayoutOptions.CenterAndExpand)
                 
                         View.Label(text="Button:")
                         View.Button(text="Decrement", command=(fun () -> dispatch Decrement), horizontalOptions=LayoutOptions.CenterAndExpand)
                
                         View.Button(text="Main page", cornerRadius=5, command=(fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions=LayoutOptions.CenterAndExpand, verticalOptions=LayoutOptions.End)
                
                      ]))

               dependsOn model.CountForActivityIndicator (fun model count -> 
                   View.ScrollingContentPage("ActivityIndicator", 
                       [View.Label(text="Label:")
                        View.Label(text= sprintf "%d" count, horizontalOptions=LayoutOptions.CenterAndExpand)
 
                        View.Label(text="ActivityIndicator (when count > 0):")
                        View.ActivityIndicator(isRunning=(count > 0), horizontalOptions=LayoutOptions.CenterAndExpand)
                  
                        View.Label(text="Button:")
                        View.Button(text="Increment", command=(fun () -> dispatch IncrementForActivityIndicator), horizontalOptions=LayoutOptions.CenterAndExpand)

                        View.Label(text="Button:")
                        View.Button(text="Decrement", command=(fun () -> dispatch DecrementForActivityIndicator), horizontalOptions=LayoutOptions.CenterAndExpand)
                 
                      ]))

               dependsOn (model.StartDate, model.EndDate) (fun model (startDate, endDate) -> 
                   View.ScrollingContentPage("DatePicker", 
                       [ View.Label(text="DatePicker (start):")
                         View.DatePicker(minimumDate= System.DateTime.Today, maximumDate=DateTime.Today + TimeSpan.FromDays(365.0), 
                             date=startDate, 
                             dateSelected=(fun args -> dispatch (StartDateSelected args.NewDate)), 
                             horizontalOptions=LayoutOptions.CenterAndExpand)

                         View.Label(text="DatePicker (end):")
                         View.DatePicker(minimumDate= startDate, maximumDate=startDate + TimeSpan.FromDays(365.0), 
                             date=endDate, 
                             dateSelected=(fun args -> dispatch (EndDateSelected args.NewDate)), 
                             horizontalOptions=LayoutOptions.CenterAndExpand)
                       ]))

               dependsOn model.EditorText (fun model editorText -> 
                   View.ScrollingContentPage("Editor", 
                       [ View.Label(text="Editor:")
                         View.Editor(text= editorText, horizontalOptions=LayoutOptions.FillAndExpand, 
                            textChanged=(fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                            completed=(fun text -> dispatch (EditorEditCompleted text)))
                       ]))

               dependsOn (model.EntryText, model.Password, model.Placeholder) (fun model (entryText, password, placeholder) -> 
                   View.ScrollingContentPage("Entry", 
                       [ View.Label(text="Entry:")
                         View.Entry(text= entryText, horizontalOptions=LayoutOptions.CenterAndExpand, 
                             textChanged=(fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                             completed=(fun text -> dispatch (EntryEditCompleted text)))

                         View.Label(text="Entry (password):")
                         View.Entry(text= password, isPassword=true, horizontalOptions=LayoutOptions.CenterAndExpand, 
                             textChanged=(fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                             completed=(fun text -> dispatch (PasswordEntryEditCompleted text)))

                         View.Label(text="Entry (placeholder):")
                         View.Entry(placeholder= placeholder, horizontalOptions=LayoutOptions.CenterAndExpand, 
                             textChanged=(fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                             completed=(fun text -> dispatch (PlaceholderEntryEditCompleted text)))

                       ]) )

               dependsOn (model.NumTaps, model.NumTaps2) (fun model (numTaps, numTaps2) -> 
                   View.ScrollingContentPage("Frame", 
                       [ View.Label(text="Frame (hasShadow=true):")
                         View.Frame(hasShadow=true, backgroundColor=Color.AliceBlue, horizontalOptions=LayoutOptions.CenterAndExpand)

                         View.Label(text="Frame (tap once gesture):")
                         View.Frame(hasShadow=true, 
                             backgroundColor=snd (pickerItems.[numTaps % pickerItems.Length]), 
                             horizontalOptions=LayoutOptions.CenterAndExpand, 
                             gestureRecognizers=[ View.TapGestureRecognizer(command=(fun () -> dispatch FrameTapped)) ] )

                         View.Label(text="Frame (tap twice gesture):")
                         View.Frame(hasShadow=true, 
                             backgroundColor=snd (pickerItems.[numTaps2 % pickerItems.Length]), 
                             horizontalOptions=LayoutOptions.CenterAndExpand, 
                             gestureRecognizers=[ View.TapGestureRecognizer(numberOfTapsRequired=2, command=(fun () -> dispatch FrameTapped2)) ] )
                 
                         View.Button(text="Main page", command=(fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions=LayoutOptions.CenterAndExpand, verticalOptions=LayoutOptions.End)
                       ]))

               dependsOn () (fun model () -> 
                   View.NonScrollingContentPage("Grid", 
                       [ View.Label(text=sprintf "Grid (6x6, auto):")
                         View.Grid(rowdefs= [for i in 1 .. 6 -> box "auto"], 
                             coldefs=[for i in 1 .. 6 -> box "auto"], 
                             children = 
                                 [ for i in 1 .. 6 do 
                                      for j in 1 .. 6 -> 
                                         let color = Color((1.0/float i), (1.0/float j), (1.0/float (i+j)), 1.0)
                                         View.BoxView(color).GridRow(i-1).GridColumn(j-1) ] )
                       ]))

           ])

        | Tabbed1 ->
           View.TabbedPage(useSafeArea=true, children=
             [
               dependsOn (model.CountForSlider, model.StepForSlider) (fun model (count, step) -> 
                  View.ScrollingContentPage("Slider", 
                     [ View.Label(text="Label:")
                       View.Label(text= sprintf "%d" count, horizontalOptions=LayoutOptions.CenterAndExpand)

                       View.Label(text="Button:")
                       View.Button(text="Increment", command=(fun () -> dispatch IncrementForSlider), horizontalOptions=LayoutOptions.CenterAndExpand)
                 
                       View.Label(text="Button:")
                       View.Button(text="Decrement", command=(fun () -> dispatch DecrementForSlider), horizontalOptions=LayoutOptions.CenterAndExpand)
                 
                       View.Label(text="Slider:")
                       View.Slider(minimum=0.0, 
                           maximum=10.0, 
                           value=double step, 
                           valueChanged=(fun args -> dispatch (SliderValueChanged (int (args.NewValue + 0.5)))), 
                           horizontalOptions=LayoutOptions.Fill) 
                    ]))

               dependsOn () (fun model () -> 
                   View.NonScrollingContentPage("Grid", 
                       [ View.Label(text=sprintf "Grid (6x6, *):")
                         View.Grid(rowdefs= [for i in 1 .. 6 -> box "*"], coldefs=[for i in 1 .. 6 -> box "*"], 
                            children = [ 
                                for i in 1 .. 6 do 
                                    for j in 1 .. 6 -> 
                                        let color = Color((1.0/float i), (1.0/float j), (1.0/float (i+j)), 1.0) 
                                        View.BoxView(color).GridRow(i-1).GridColumn(j-1) ] )
                         View.Button(text="Main page", 
                             command=(fun () -> dispatch (SetRootPageKind (Choice false))), 
                             horizontalOptions=LayoutOptions.CenterAndExpand, verticalOptions=LayoutOptions.End)
                
                        ]))

               dependsOn (model.GridSize, model.NewGridSize) (fun model (gridSize, newGridSize) -> 
                  View.NonScrollingContentPage("Grid+Pinch", 
                      [ View.Label(text=sprintf "Grid (nxn, pinch, size = %f):" newGridSize)
                        // The Grid doesn't change during the pinch...
                        dependsOn gridSize (fun _ _ -> 
                          View.Grid(rowdefs= [for i in 1 .. gridSize -> box "*"], coldefs=[for i in 1 .. gridSize -> box "*"], 
                              children = [ 
                                  for i in 1 .. gridSize do 
                                      for j in 1 .. gridSize -> 
                                         let color = Color((1.0/float i), (1.0/float j), (1.0/float (i+j)), 1.0) 
                                         View.BoxView(color).GridRow(i-1).GridColumn(j-1) ]))
                      ], 
                      gestureRecognizers=[ View.PinchGestureRecognizer(pinchUpdated=(fun pinchArgs -> 
                                              dispatch (UpdateNewGridSize (pinchArgs.Scale, pinchArgs.Status)))) ] ))

               dependsOn model.GridPortal (fun model gridPortal -> 
                  let dx, dy = gridPortal
                  View.NonScrollingContentPage("Grid+Pan", 
                      children=
                          [ View.Label(text= sprintf "Grid (nxn, auto, edit entries, 1-touch pan, (%d, %d):" dx dy)
                            View.Grid(rowdefs= [for row in 1 .. 6 -> box "*"], coldefs=[for col in 1 .. 6 -> box "*"], 
                               children = [ for row in 1 .. 6 do 
                                               for col in 1 .. 6 ->
                                                  let item = View.Label(text=sprintf "(%d, %d)" (col+dx) (row+dy), backgroundColor=Color.White, textColor=Color.Black) 
                                                  item.GridRow(row-1).GridColumn(col-1) ])
                      ], 
                      gestureRecognizers=[ View.PanGestureRecognizer(touchPoints=1, panUpdated=(fun panArgs -> 
                                              if panArgs.StatusType = GestureStatus.Running then 
                                                  dispatch (UpdateGridPortal (dx - int (panArgs.TotalX/10.0), dy - int (panArgs.TotalY/10.0))))) ] ))

               dependsOn () (fun model () -> 
                 View.NonScrollingContentPage("Image", 
                     [ View.Label(text="Image:")
                       View.Image(source="http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg", 
                           horizontalOptions=LayoutOptions.FillAndExpand,
                           verticalOptions=LayoutOptions.FillAndExpand) ]))
             ])

        | Tabbed2 ->
           View.TabbedPage(useSafeArea=true, children=
             [
               dependsOn (model.PickedColorIndex) (fun model (pickedColorIndex) -> 
                  View.ScrollingContentPage("Picker", 
                     [ View.Label(text="Picker:")
                       View.Picker(title="Choose Color:", textColor=snd pickerItems.[pickedColorIndex], selectedIndex=pickedColorIndex, itemsSource=(Array.map fst pickerItems), horizontalOptions=LayoutOptions.CenterAndExpand, selectedIndexChanged=(fun (i, item) -> dispatch (PickerItemChanged i)))
                     ]))
                      
               dependsOn () (fun model () -> 
                  View.ScrollingContentPage("ListView", 
                     [ View.Label(text="ListView:")
                       View.ListView(
                           items = [ 
                               for i in 0 .. 10 do 
                                   yield View.Label "Ionide"
                                   yield View.Label(formattedText=View.FormattedString([|View.Span(text="Visual ", backgroundColor=Color.Green); View.Span(text="Studio ", fontSize = 10)|]))
                                   yield View.Label "Emacs"
                                   yield View.Label(formattedText=View.FormattedString([|View.Span(text="Visual ", fontAttributes=FontAttributes.Bold); View.Span(text="Studio ", fontAttributes=FontAttributes.Italic); View.Span(text="Code", foregroundColor = Color.Blue)|]))
                                   yield View.Label "Rider"], 
                           horizontalOptions=LayoutOptions.CenterAndExpand, 
                           itemSelected=(fun idx -> dispatch (ListViewSelectedItemChanged idx)))
                ]))

                      
               dependsOn (model.SearchTerm) (fun model (searchTerm) -> 
                  View.ScrollingContentPage("SearchBar", 
                     [ View.Label(text="SearchBar:")
                       View.SearchBar(
                            placeholder = "Enter search term",
                            searchCommand = (fun searchBarText -> dispatch (ExecuteSearch searchBarText)),
                            canExecute=true) 
                       View.Label(text="You searched for " + searchTerm) ]))

               dependsOn () (fun model () -> 
                   View.ScrollingContentPage("ListViewGrouped", 
                       [ View.Label(text="ListView (grouped):")
                         View.ListViewGrouped(
                             items= 
                                [ View.Label "Europe", [ View.Label "Russia"; View.Label "Germany"; View.Label "Poland"; View.Label "Greece"   ]
                                  View.Label "Asia", [ View.Label "China"; View.Label "Japan"; View.Label "North Korea"; View.Label "South Korea"   ]
                                  View.Label "Australasia", [ View.Label "Australia"; View.Label "New Zealand"; View.Label "Fiji" ] ], 
                             horizontalOptions=LayoutOptions.CenterAndExpand, 
                             isGroupingEnabled=true, 
                             itemSelected=(fun idx -> dispatch (ListViewGroupedSelectedItemChanged idx)))
                   ]))

             ])
        | Tabbed3 ->
           View.TabbedPage(useSafeArea=true, 
            children=
             [ 
               dependsOn model.Count (fun model count -> 
                   View.ContentPage(title="FlexLayout", useSafeArea=true,
                       padding = new Thickness (10.0, 20.0, 10.0, 5.0), 
                       content= 
                           View.ScrollView(orientation=ScrollOrientation.Both,
                              content = View.FlexLayout(
                                  children = [
                                      View.Frame(heightRequest=480.0, widthRequest=300.0, 
                                          content = View.FlexLayout( direction=FlexDirection.Column,
                                              children = [ 
                                                  View.Label(text="Seated Monkey", margin=Thickness(0.0, 8.0), fontSize="Large", textColor=Color.Blue)
                                                  View.Label(text="This monkey is laid back and relaxed, and likes to watch the world go by.", margin=Thickness(0.0, 4.0), textColor=Color.Black)
                                                  View.Label(text="  • Often smiles mysteriously", margin=Thickness(0.0, 4.0), textColor=Color.Black)
                                                  View.Label(text="  • Sleeps sitting up", margin=Thickness(0.0, 4.0), textColor=Color.Black)
                                                  View.Image(heightRequest=240.0, 
                                                      widthRequest=160.0, 
                                                      source="https://upload.wikimedia.org/wikipedia/commons/thumb/6/66/Vervet_monkey_Krugersdorp_game_reserve_%285657678441%29.jpg/160px-Vervet_monkey_Krugersdorp_game_reserve_%285657678441%29.jpg"
                                                  ).FlexOrder(-1).FlexAlignSelf(FlexAlignSelf.Center)
                                                  View.Label(margin=Thickness(0.0, 4.0)).FlexGrow(1.0)
                                                  View.Button(text="Learn More", fontSize="Large", textColor=Color.White, backgroundColor=Color.Green, cornerRadius=20) ]),
                                          backgroundColor=Color.LightYellow,
                                          borderColor=Color.Blue,
                                          margin=10.0,
                                          cornerRadius=15.0)
                                      View.Frame(heightRequest=480.0, widthRequest=300.0, 
                                          content = View.FlexLayout( direction=FlexDirection.Column,
                                              children = [ 
                                                  View.Label(text="Banana Monkey", margin=Thickness(0.0, 8.0), fontSize="Large", textColor=Color.Blue)
                                                  View.Label(text="Watch this monkey eat a giant banana.", margin=Thickness(0.0, 4.0), textColor=Color.Black)
                                                  View.Label(text="  • More fun than a barrel of monkeys", margin=Thickness(0.0, 4.0), textColor=Color.Black)
                                                  View.Label(text="  • Banana not included", margin=Thickness(0.0, 4.0), textColor=Color.Black)
                                                  View.Image(heightRequest=213.0, 
                                                      widthRequest=320.0, 
                                                      source="https://upload.wikimedia.org/wikipedia/commons/thumb/c/c1/Crab_eating_macaque_in_Ubud_with_banana.JPG/320px-Crab_eating_macaque_in_Ubud_with_banana.JPG"
                                                  ).FlexOrder(-1).FlexAlignSelf(FlexAlignSelf.Center)
                                                  View.Label(margin=Thickness(0.0, 4.0)).FlexGrow(1.0)
                                                  View.Button(text="Learn More", fontSize="Large", textColor=Color.White, backgroundColor=Color.Green, cornerRadius=20) ]),
                                          backgroundColor=Color.LightYellow,
                                          borderColor=Color.Blue,
                                          margin=10.0,
                                          cornerRadius=15.0)
                                  ] ))
                           ) )

               dependsOn () (fun model () -> 
                View.ScrollingContentPage("TableView", 
                 [View.Label(text="TableView:")
                  View.TableView(items= [ ("Videos", [ View.SwitchCell(on=true, text="Luca 2008", onChanged=(fun args -> ()) ) 
                                                       View.SwitchCell(on=true, text="Don 2010", onChanged=(fun args -> ()) ) ] )
                                          ("Books", [ View.SwitchCell(on=true, text="Expert F#", onChanged=(fun args -> ()) ) 
                                                      View.SwitchCell(on=false, text="Programming F#", onChanged=(fun args -> ()) ) ])
                                          ("Contact", [ View.EntryCell(label="Email", placeholder="foo@bar.com", completed=(fun args -> ()) )
                                                        View.EntryCell(label="Phone", placeholder="+44 87654321", completed=(fun args -> ()) )] )], 
                                  horizontalOptions=LayoutOptions.StartAndExpand) 
                    ]))

               dependsOn model.Count (fun model count -> 
                 View.ContentPage(title="RelativeLayout", 
                  padding = new Thickness (10.0, 20.0, 10.0, 5.0), 
                  content= View.RelativeLayout(
                      children=[ 
                          View.Label(text = "RelativeLayout Example", textColor = Color.Red)
                                .XConstraint(Constraint.RelativeToParent(fun parent -> 0.0))
                          View.Label(text = "Positioned relative to my parent", textColor = Color.Red)
                                .XConstraint(Constraint.RelativeToParent(fun parent -> parent.Width / 3.0))
                                .YConstraint(Constraint.RelativeToParent(fun parent -> parent.Height / 2.0))
                      ])))


               dependsOn model.Count (fun model count -> 
                   View.ContentPage(title="AbsoluteLayout", useSafeArea=true,
                       padding = new Thickness (10.0, 20.0, 10.0, 5.0), 
                       content= View.StackLayout(
                           children=[ 
                               View.Label(text = "AbsoluteLayout Demo", fontSize = Device.GetNamedSize(NamedSize.Large, typeof<Label>), horizontalOptions = LayoutOptions.Center)
                               View.AbsoluteLayout(backgroundColor = Color.Blue.WithLuminosity(0.9), 
                                   verticalOptions = LayoutOptions.FillAndExpand, 
                                   children = [
                                      View.Label(text = "Top Left", textColor = Color.Black)
                                          .LayoutFlags(AbsoluteLayoutFlags.PositionProportional)
                                          .LayoutBounds(Rectangle(0.0, 0.0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize))
                                      View.Label(text = "Centered", textColor = Color.Black)
                                          .LayoutFlags(AbsoluteLayoutFlags.PositionProportional)
                                          .LayoutBounds(Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize))
                                      View.Label(text = "Bottom Right", textColor = Color.Black)
                                          .LayoutFlags(AbsoluteLayoutFlags.PositionProportional)
                                          .LayoutBounds(Rectangle(1.0, 1.0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize)) ])
                            ])))

                ])
         
         | Navigation -> 

         // NavigationPage example
           dependsOn model.PageStack (fun model pageStack -> 
              View.NavigationPage(pages=
                      [ for page in List.rev pageStack do
                          match page with 
                          | Some "Home" -> 
                              yield 
                                  View.ContentPage(useSafeArea=true,
                                    content=View.StackLayout(
                                     children=
                                       [ View.Label(text="Home Page", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center)
                                         View.Button(text="Push Page A", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "A")))
                                         View.Button(text="Push Page B", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "B")))
                
                                         View.Button(text="Main page", textColor=Color.White, backgroundColor=Color.Navy, command=(fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions=LayoutOptions.CenterAndExpand, verticalOptions=LayoutOptions.End)
                                        ]) ).HasNavigationBar(true).HasBackButton(false)
                          | Some "A" -> 
                              yield 
                                View.ContentPage(useSafeArea=true,
                                    content=
                                     View.StackLayout(
                                      children=
                                       [View.Label(text="Page A", verticalOptions=LayoutOptions.Center, horizontalOptions=LayoutOptions.Center)
                                        View.Button(text="Page B", verticalOptions=LayoutOptions.Center, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "B")))
                                        View.Button(text="Page C", verticalOptions=LayoutOptions.Center, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "C")))
                                        View.Button(text="Replace by Page B", verticalOptions=LayoutOptions.Center, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (ReplacePage "B")))
                                        View.Button(text="Replace by Page C", verticalOptions=LayoutOptions.Center, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (ReplacePage "C")))
                                        View.Button(text="Back", verticalOptions=LayoutOptions.Center, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch PopPage ))
                                        ]) ).HasNavigationBar(true).HasBackButton(true)
                          | Some "B" -> 
                              yield 
                                View.ContentPage(useSafeArea=true,
                                    content=View.StackLayout(
                                         children=
                                             [View.Label(text="Page B", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center)
                                              View.Label(text="(nb. no back button in navbar)", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center)
                                              View.Button(text="Page A", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "A")))
                                              View.Button(text="Page C", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "C")))
                                              View.Button(text="Back", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch PopPage ))
                                             ]) ).HasNavigationBar(true).HasBackButton(false)
                          | Some "C" -> 
                              yield 
                                View.ContentPage(useSafeArea=true,
                                    content=View.StackLayout(
                                      children=
                                       [View.Label(text="Page C", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center)
                                        View.Label(text="(nb. no navbar)", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center)
                                        View.Button(text="Page A", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "A")))
                                        View.Button(text="Page B", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "B")))
                                        View.Button(text="Back", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch PopPage ))
                                        ]) ).HasNavigationBar(false).HasBackButton(false)

                          | _ -> 
                               ()  ], 
                     popped=(fun args -> dispatch PagePopped) , 
                     poppedToRoot=(fun args -> dispatch GoHomePage)  ))

        | MasterDetail -> 
         // MasterDetail where the Master acts as a hamburger-style menu
          dependsOn (model.DetailPage, model.IsMasterPresented) (fun model (detailPage, isMasterPresented) -> 
            View.MasterDetailPage(
               masterBehavior=MasterBehavior.Popover, 
               isPresented=isMasterPresented, 
               isPresentedChanged=(fun b -> dispatch (IsMasterPresentedChanged b)), 
               master = 
                 View.ContentPage(useSafeArea=true, title="Master", 
                  content = 
                    View.StackLayout(backgroundColor=Color.Gray, 
                      children=[ View.Button(text="Detail A", textColor=Color.White, backgroundColor=Color.Navy, command=(fun () -> dispatch (SetDetailPage "A")))
                                 View.Button(text="Detail B", textColor=Color.White, backgroundColor=Color.Navy, command=(fun () -> dispatch (SetDetailPage "B")))
                                 View.Button(text="Main page", textColor=Color.White, backgroundColor=Color.Navy, command=(fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions=LayoutOptions.CenterAndExpand, verticalOptions=LayoutOptions.End) 
                               ]) ), 
               detail = 
                 View.NavigationPage( 
                   pages=[
                     View.ContentPage(title="Detail", useSafeArea=true,
                      content = 
                        View.StackLayout(backgroundColor=Color.Gray, 
                          children=[ View.Label(text="Detail " + detailPage, textColor=Color.White, backgroundColor=Color.Navy)
                                     View.Button(text="Main page", textColor=Color.White, backgroundColor=Color.Navy, command=(fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions=LayoutOptions.CenterAndExpand, verticalOptions=LayoutOptions.End)  ]) 
                          ).HasNavigationBar(true).HasBackButton(true) ], 
                   poppedToRoot=(fun args -> dispatch (IsMasterPresentedChanged true) ) ) ) )

         | InfiniteScrollList -> 
              dependsOn (model.InfiniteScrollMaxRequested ) (fun model max -> 
               View.ScrollingContentPage("ListView (InfiniteScrollList)", 
                [View.Label(text="InfiniteScrollList:")
                 View.ListView(items = [ for i in 1 .. max do 
                                           yield dependsOn i (fun _ i -> View.Label("Item " + string i, textColor=(if i % 3 = 0 then Color.CadetBlue else Color.LightCyan))) ], 
                               horizontalOptions=LayoutOptions.CenterAndExpand, 
                               // Every time the last element is needed, grow the set of data to be at least 10 bigger then that index 
                               itemAppearing=(fun idx -> if idx >= max - 2 then dispatch (SetInfiniteScrollMaxIndex (idx + 10) ) )  )
                 View.Button(text="Main page", command=(fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions=LayoutOptions.CenterAndExpand, verticalOptions=LayoutOptions.End)
                
                ]))


type App () as app = 
    inherit Application ()

    let runner = 
        Program.mkSimple App.init App.update App.view
        |> Program.withConsoleTrace
        |> Program.runWithDynamicView app
