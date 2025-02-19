module MarkdownCyoa.Core.Parser.ParagraphId.Tests
open Expecto
open Farkdown.Helpers

[<Tests>]
let parseTest =
    testList "MarkdownCyoa.Core.Parser.ParagraphId.parse" [
        testCase "hello, world" <| fun () ->
            Expect.equal
                (parse [text "hello, world"])
                "hello-world"
                ""
        testCase "**hello**, world" <| fun () ->
            Expect.equal
                (parse [bold (text "hello"); text ", world"])
                "hello-world"
                ""
    ]
