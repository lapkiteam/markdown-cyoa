module MarkdownCyoa.Core.Parser.Action.Tests
open Expecto
open Farkdown.Helpers

[<Tests>]
let parseTest =
    testList "MarkdownCyoa.Core.Parser.Action.parse" [
        testCase "[drink potion](#drinking-potion)" <| fun () ->
            Expect.equal
                (parse ([ url "#drinking-potion" "" [ text "drink potion" ] ], []))
                (Some {
                    Description = [text "drink potion"]
                    Reference = "drinking-potion"
                })
                ""
        testCase "[drink] potion(#drinking-potion)" <| fun () ->
            Expect.equal
                (parse ([
                    url "#drinking-potion" "" [ text "drink" ]
                    text " potion"
                ], []))
                (Some {
                    Description = [text "drink"]
                    Reference = "drinking-potion"
                })
                ""
        testCase "drink [potion](#drinking-potion)" <| fun () ->
            Expect.equal
                (parse ([
                    text "drink "
                    url "#drinking-potion" "" [ text "potion" ]
                ], []))
                (Some {
                    Description = [text "potion"]
                    Reference = "drinking-potion"
                })
                ""
    ]
