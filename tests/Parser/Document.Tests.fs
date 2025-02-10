module MarkdownCyoa.Core.Parser.Document.Tests
open Expecto
open Farkdown.Helpers

open MarkdownCyoa.Core.Tests

[<Tests>]
let parseTest =
    testList "Document.Parser.parse" [
        testCase "1" <| fun () ->
            Expect.equal
                (parse (String.concat "\n" [
                    "# first paragraph"
                    "some text"
                    ""
                    "* [choice 1](#first-paragraph)"
                    "* [choice 2](#paragraph2)"
                ]))
                (Ok [
                    {
                        Id = ""
                        Title = [ text "first paragraph" ]
                        Main = [
                            p [[ text "some text" ]]
                        ]
                        Actions = [
                            {
                                Description = [ text "choice 1" ]
                                Reference = "#first-paragraph"
                            }
                            {
                                Description = [ text "choice 2" ]
                                Reference = "#paragraph2"
                            }
                        ]
                    }
                ])
                ""
        testCase "cinderella cyoa" <| fun () ->
            Expect.equal
                (parse CinderellaCyoa.rawAll)
                (Ok CinderellaCyoa.parsedAll)
                ""
    ]
