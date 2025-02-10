module MarkdownCyoa.Core.Document.Parser.Tests
open Expecto

module CinderellaCyoa =
    open Farkdown.Helpers

    open MarkdownCyoa.Core

    let rawBegin =
        [
            "## Begin"
            ""
            "Cinderella stood in the kitchen, her heart heavy with longing. The royal ball was tonight, and she desperately wanted to go. But her stepmother had left her with a mountain of chores. \"If you finish everything by sunset,\" her stepmother had sneered, \"you may go.\""
            ""
            "Cinderella glanced at the clock. She had only a few hours to clean the entire house, mend her stepsisters' dresses, and prepare dinner."
            ""
            "---"
            ""
            "**What should Cinderella do first?**"
            ""
            "* [Start sweeping the floors.](#sweep-floors)"
            "* [Begin mending the dresses.](#mend-dresses)"
            "* [Ask the mice for help.](#ask-mice)"
        ] |> String.concat "\n"

    let parsedBegin =
        {
            Id = ""
            Title = [ text "Begin" ]
            Main = [
                p [[
                    text "Cinderella stood in the kitchen, her heart heavy with longing. The royal ball was tonight, and she desperately wanted to go. But her stepmother had left her with a mountain of chores. \"If you finish everything by sunset,\" her stepmother had sneered, \"you may go.\""
                ]]
                p [[
                    text "Cinderella glanced at the clock. She had only a few hours to clean the entire house, mend her stepsisters' dresses, and prepare dinner."
                ]]
                p [[
                    text "---"
                ]]
                p [[
                    text "**What should Cinderella do first?**"
                ]]
            ]
            Actions = [
                {
                    Description = [ text "Start sweeping the floors." ]
                    Reference = "#sweep-floors"
                }
                {
                    Description = [ text "Begin mending the dresses." ]
                    Reference = "#mend-dresses"
                }
                {
                    Description = [ text "Ask the mice for help." ]
                    Reference = "#ask-mice"
                }
            ]
        }

    let rawSweepFloors =
        [
            "## Sweep Floors"
            ""
            "Cinderella grabbed her broom and began sweeping the dusty floors. As she worked, she hummed a tune to keep her spirits up."
            ""
            "Suddenly, she noticed a glint under the couch. It was a silver button from one of her stepsisters' dresses! If she didn't sew it back on, her stepsister would be furious."
            ""
            "---"
            ""
            "**What should Cinderella do now?**"
            ""
            "* [Stop sweeping and sew the button back on.](#sew-button)"
            "* [Keep sweeping and deal with the button later.](#keep-sweeping)"
        ] |> String.concat "\n"

    let rawMendDresses =
        [
            "## Mend Dresses"
            ""
            "Cinderella sat down with the torn dresses and began stitching. Her fingers moved quickly, but there were so many rips and tears."
            ""
            "As she worked, she heard the clock chime. Time was running out! She still needed to clean the house and prepare dinner."
            ""
            "---"
            ""
            "**What should Cinderella do now?**"
            ""
            "* [Finish mending the dresses first.](#finish-mending)"
            "* [Put the dresses aside and start cleaning.](#start-cleaning)"
            "* [Put the dresses aside and start dinner.](#start-cleaning)"
        ] |> String.concat "\n"

    let rawAll =
        [
            rawBegin
            rawSweepFloors
            rawMendDresses
        ] |> String.concat "\n"

[<Tests>]
let parseTest =
    testList "Document.Parser.parse" [
        testCase "1" <| fun () ->
            Expect.equal
                // (parser (String.concat "\n" [
                //     "# first paragraph"
                //     "some text"
                //     ""
                //     "* [choice 1](first-paragraph)"
                //     "* [choice 2](first-paragraph2)"
                // ]))
                (parse rawDoc)
                (Ok [
                    {
                        Id = ""
                        Title = []
                        Main = []
                        Actions = []
                    }
                ])
                ""
    ]
