module MarkdownCyoa.Core.Mermaid.Tests
open Expecto
open FMermaid.Api.Flowchart

open MarkdownCyoa.Core.Tests

[<Tests>]
let toMermaidTest =
    testList "MarkdownCyoa.Core.Mermaid.toMermaid" [
        testCase "base" <| fun () ->
            Expect.equal
                (toMermaid CinderellaCyoa.parsedAll)
                (Document.create [
                    Statement.Connection {
                        FromNode = "begin"
                        EdgeLabel = Some "Start sweeping the floors."
                        ToNode = "sweep-floors"
                    }
                    Statement.Connection {
                        FromNode = "begin"
                        EdgeLabel = Some "Begin mending the dresses."
                        ToNode = "mend-dresses"
                    }
                    Statement.Connection {
                        FromNode = "begin"
                        EdgeLabel = Some "Ask the mice for help."
                        ToNode = "ask-mice"
                    }
                    Statement.Connection {
                        FromNode = "sweep-floors"
                        EdgeLabel = Some "Stop sweeping and sew the button back on."
                        ToNode = "sew-button"
                    }
                    Statement.Connection {
                        FromNode = "sweep-floors"
                        EdgeLabel = Some "Keep sweeping and deal with the button later."
                        ToNode = "keep-sweeping"
                    }
                    Statement.Connection {
                        FromNode = "mend-dresses"
                        EdgeLabel = Some "Finish mending the dresses first."
                        ToNode = "finish-mending"
                    }
                    Statement.Connection {
                        FromNode = "mend-dresses"
                        EdgeLabel = Some "Put the dresses aside and start cleaning."
                        ToNode = "start-cleaning"
                    }
                    Statement.Connection {
                        FromNode = "mend-dresses"
                        EdgeLabel = Some "Put the dresses aside and start dinner."
                        ToNode = "start-cleaning"
                    }
                ])
                ""
    ]
