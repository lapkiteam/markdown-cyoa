module MarkdownCyoa.Core.Qsp.Tests
open Expecto
open Qsp.Ast
open Qsp.Printer
open Qsp.Printer.Ast

open MarkdownCyoa.Core.Tests

module LineKind =
    open LineKind
    open Farkdown.Helpers

    module Tests =
        [<Tests>]
        let ofFarkdownLineElementTest =
            testList "MarkdownCyoa.Core.LineKind.ofFarkdownLineElement" [
                testCase "link with image" <| fun () ->
                    Expect.equal
                        (ofFarkdownLineElement Line.ofFarkdownLine (
                            url "http://example.com" "title" [
                                img "http://example.com/image.jpg" "image" "image-alt"
                            ]
                        ))
                        (LineKind.StringKind "<a href=\"http://example.com\" title=\"title\"><img src=\"http://example.com/image.jpg\" alt=\"image-alt\" title=\"image\" /></a>")
                        ""
                testCase "link with text" <| fun () ->
                    Expect.equal
                        (ofFarkdownLineElement Line.ofFarkdownLine (
                            url "http://example.com" "title" [
                                text "click me!"
                            ]
                        ))
                        (LineKind.StringKind "<a href=\"http://example.com\" title=\"title\">click me!</a>")
                        ""
            ]

[<Tests>]
let toMermaidTest =
    testList "MarkdownCyoa.Core.Qsp.toQsp" [
        testCase "base" <| fun () ->
            Expect.equal
                (
                    toQsp CinderellaCyoa.parsedAll
                    |> Document.print
                        (IndentsOption.UsingSpaces 2)
                        FormatConfig.Default
                )
                (String.concat "\n" [
                    String.concat System.Environment.NewLine [
                        "# location-1"
                        "if $args[0] = 'chapter-1':"
                        "  if $args[1] = 'begin':"
                        "    'Cinderella stood in the kitchen, her heart heavy with longing. The royal ball was tonight, and she desperately wanted to go. But her stepmother had left her with a mountain of chores. \"If you finish everything by sunset,\" her stepmother had sneered, \"you may go.\"'"
                        "    ''"
                        "    'Cinderella glanced at the clock. She had only a few hours to clean the entire house, mend her stepsisters'' dresses, and prepare dinner.'"
                        "    ''"
                        "    '---'"
                        "    ''"
                        "    '**What should Cinderella do first?**'"
                        "    ''"
                        "    act 'Start sweeping the floors.': gt $curLoc, 'chapter-1', 'sweep-floors'"
                        "    act 'Begin mending the dresses.': gt $curLoc, 'chapter-1', 'mend-dresses'"
                        "    act 'Ask the mice for help.': gt $curLoc, 'chapter-1', 'ask-mice'"
                        "  elseif $args[1] = 'sweep-floors':"
                        "    'Cinderella grabbed her broom and began sweeping the dusty floors. As she worked, she hummed a tune to keep her spirits up.'"
                        "    ''"
                        "    'Suddenly, she noticed a glint under the couch. It was a silver button from one of her stepsisters'' dresses! If she didn''t sew it back on, her stepsister would be furious.'"
                        "    ''"
                        "    '---'"
                        "    ''"
                        "    '**What should Cinderella do now?**'"
                        "    ''"
                        "    act 'Stop sweeping and sew the button back on.': gt $curLoc, 'chapter-1', 'sew-button'"
                        "    act 'Keep sweeping and deal with the button later.': gt $curLoc, 'chapter-1', 'keep-sweeping'"
                        "  elseif $args[1] = 'mend-dresses':"
                        "    'Cinderella sat down with the torn dresses and began stitching. Her fingers moved quickly, but there were so many rips and tears.'"
                        "    ''"
                        "    'As she worked, she heard the clock chime. Time was running out! She still needed to clean the house and prepare dinner.'"
                        "    ''"
                        "    '---'"
                        "    ''"
                        "    '**What should Cinderella do now?**'"
                        "    ''"
                        "    act 'Finish mending the dresses first.': gt $curLoc, 'chapter-1', 'finish-mending'"
                        "    act 'Put the dresses aside and start cleaning.': gt $curLoc, 'chapter-1', 'start-cleaning'"
                        "    act 'Put the dresses aside and start dinner.': gt $curLoc, 'chapter-1', 'start-cleaning'"
                        "  end"
                        "end"
                        "--- location-1 ----------"
                    ]
                ])
                ""
    ]
