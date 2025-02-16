module MarkdownCyoa.Core.Tests.CinderellaCyoa
open Farkdown.Helpers

open MarkdownCyoa.Core

let rawLocationOne =
    [
        "# Location 1"
    ]
    |> String.concat "\n"

let rawChapterOne =
    [
        "## Chapter 1"
    ]
    |> String.concat "\n"

let rawBegin =
    [
        "### Begin"
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
        Id = "begin"
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
                Reference = "sweep-floors"
            }
            {
                Description = [ text "Begin mending the dresses." ]
                Reference = "mend-dresses"
            }
            {
                Description = [ text "Ask the mice for help." ]
                Reference = "ask-mice"
            }
        ]
    }

let rawSweepFloors =
    [
        "### Sweep Floors"
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

let parsedSweepFloors =
    {
        Id = "sweep-floors"
        Title = [ text "Sweep Floors" ]
        Main = [
            p [[
                text "Cinderella grabbed her broom and began sweeping the dusty floors. As she worked, she hummed a tune to keep her spirits up."
            ]]
            p [[
                text "Suddenly, she noticed a glint under the couch. It was a silver button from one of her stepsisters' dresses! If she didn't sew it back on, her stepsister would be furious."
            ]]
            p [[ text "---" ]]
            p [[ text "**What should Cinderella do now?**" ]]
        ]
        Actions = [
            {
                Reference = "sew-button"
                Description = [text "Stop sweeping and sew the button back on."]
            }
            {
                Reference = "keep-sweeping"
                Description = [text "Keep sweeping and deal with the button later."]
            }
        ]
    }

let rawMendDresses =
    [
        "### Mend Dresses"
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

let parsedMendDresses =
    {
        Id = "mend-dresses"
        Title = [ text "Mend Dresses" ]
        Main = [
            p [[
                text "Cinderella sat down with the torn dresses and began stitching. Her fingers moved quickly, but there were so many rips and tears."
            ]]
            p [[
                text "As she worked, she heard the clock chime. Time was running out! She still needed to clean the house and prepare dinner."
            ]]
            p [[text "---"]];
            p [[text "**What should Cinderella do now?**"]]
        ]
        Actions = [
            {
                Reference = "finish-mending"
                Description = [text "Finish mending the dresses first."]
            }
            {
                Reference = "start-cleaning"
                Description = [text "Put the dresses aside and start cleaning."]
            }
            {
                Reference = "start-cleaning"
                Description = [text "Put the dresses aside and start dinner."]
            }
        ]
    }

let rawAll =
    [
        rawLocationOne
        rawChapterOne
        rawBegin
        rawSweepFloors
        rawMendDresses
    ] |> String.concat "\n"

let parsedAll: MarkdownCyoa.Core.Document =
    [
        Location.create "location-1" [text "Location 1"] [
            Scene.create "chapter-1" [text "Chapter 1"]  [
                parsedBegin
                parsedSweepFloors
                parsedMendDresses
            ]
        ]
    ]
