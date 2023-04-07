using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.RPG.Checks {
    public enum OpposedOutcomeDirection {
        Proponent,
        Opponent,
        Stalemate
    }

    public enum OpposedOutcomeDegree {
        None = 0,
        Marginal,
        Minor,
        Moderate,
        Major,
        Total
    }

    public static class OpposedOutcomeExtensions {
        public static string Describe(this OpposedOutcomeDirection direction) {
            switch (direction) {
                case OpposedOutcomeDirection.Proponent:
                    return "Proponent";
                case OpposedOutcomeDirection.Opponent:
                    return "Opponent";
                case OpposedOutcomeDirection.Stalemate:
                    return "Stalemate";
            }
            return "Unknown";
        }

        public static string Describe(this OpposedOutcomeDegree degree) {
            switch (degree) {
                case OpposedOutcomeDegree.None:
                    return "None";
                case OpposedOutcomeDegree.Marginal:
                    return "Marginal";
                case OpposedOutcomeDegree.Minor:
                    return "Minor";
                case OpposedOutcomeDegree.Moderate:
                    return "Moderate";
                case OpposedOutcomeDegree.Major:
                    return "Major";
                case OpposedOutcomeDegree.Total:
                    return "Total";
                default:
                    return "Unknown";
            }
        }

        public static string DescribeColour(this OpposedOutcomeDegree degree)
        {
            switch (degree)
            {
                case OpposedOutcomeDegree.None:
                    return "None".Colour(Telnet.Yellow);
                case OpposedOutcomeDegree.Marginal:
                    return "Marginal".Colour(Telnet.Green);
                case OpposedOutcomeDegree.Minor:
                    return "Minor".Colour(Telnet.Green);
                case OpposedOutcomeDegree.Moderate:
                    return "Moderate".Colour(Telnet.Green);
                case OpposedOutcomeDegree.Major:
                    return "Major".Colour(Telnet.BoldGreen);
                case OpposedOutcomeDegree.Total:
                    return "Total".Colour(Telnet.BoldGreen);
                default:
                    return "Unknown".Colour(Telnet.BoldMagenta);
            }
        }

        public static bool TryParse(string text, out OpposedOutcomeDegree degree) {
            switch (text) {
                case "None":
                    degree = OpposedOutcomeDegree.None;
                    break;
                case "Marginal":
                    degree = OpposedOutcomeDegree.Marginal;
                    break;
                case "Minor":
                    degree = OpposedOutcomeDegree.Minor;
                    break;
                case "Moderate":
                    degree = OpposedOutcomeDegree.Moderate;
                    break;
                case "Major":
                    degree = OpposedOutcomeDegree.Major;
                    break;
                case "Total":
                    degree = OpposedOutcomeDegree.Total;
                    break;
                default:
                    degree = OpposedOutcomeDegree.None;
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     An opposed Outcome is designed to accept the Outcome of two rolls made against one another, and gives an indication
    ///     of who "Won" and by how much.
    /// </summary>
    public class OpposedOutcome {
        public OpposedOutcome(Outcome proponent, Outcome opponent) {
            var difference = (int) proponent - (int) opponent;
            if (difference == 0) {
                Outcome = OpposedOutcomeDirection.Stalemate;
                Degree = OpposedOutcomeDegree.None;
            }
            else {
                Outcome = difference > 0 ? OpposedOutcomeDirection.Proponent : OpposedOutcomeDirection.Opponent;
                Degree = (OpposedOutcomeDegree) Math.Min(Math.Abs(difference), 5);
            }
        }

        public OpposedOutcome(OpposedOutcomeDirection outcome, OpposedOutcomeDegree degree) {
            Outcome = outcome;
            Degree = degree;
        }

        public OpposedOutcome(IReadOnlyDictionary<Difficulty,CheckOutcome> proponentOutcomes,
                              IReadOnlyDictionary<Difficulty, CheckOutcome> opponentOutcomes, 
                              Difficulty primaryProponent, 
                              Difficulty primaryOpponent) {
            var proponentDifficulty = primaryProponent;
            var opponentDifficulty = primaryOpponent;
            var increaseDifficulty = default(bool?);
            while (true) {
                var proponent = proponentOutcomes[proponentDifficulty].Outcome;
                var opponent = opponentOutcomes[opponentDifficulty].Outcome;
                var difference = (int)proponent - (int)opponent;
                if (difference == 0)
                {
                    increaseDifficulty ??= !proponent.IsFail();
                    if (!increaseDifficulty.Value) {
                        if (proponentDifficulty == Difficulty.Automatic && opponentDifficulty == Difficulty.Automatic) {
                            if (Constants.Random.Next(0, 2) == 0) {
                                Outcome = OpposedOutcomeDirection.Proponent;
                                Degree = OpposedOutcomeDegree.Marginal;
                            }
                            else {
                                Outcome = OpposedOutcomeDirection.Opponent;
                                Degree = OpposedOutcomeDegree.Marginal;
                            }
                            ProponentDifficulty = Difficulty.Automatic;
                            OpponentDifficulty = Difficulty.Automatic;
                            break;
                        }
                        proponentDifficulty = proponentDifficulty.StageDown(1);
                        opponentDifficulty = opponentDifficulty.StageDown(1);
                    }
                    else {
                        if (proponentDifficulty == Difficulty.Impossible && opponentDifficulty == Difficulty.Impossible)
                        {
                            if (Constants.Random.Next(0, 2) == 0)
                            {
                                Outcome = OpposedOutcomeDirection.Proponent;
                                Degree = OpposedOutcomeDegree.Marginal;
                            }
                            else
                            {
                                Outcome = OpposedOutcomeDirection.Opponent;
                                Degree = OpposedOutcomeDegree.Marginal;
                            }

                            ProponentDifficulty = Difficulty.Impossible;
                            OpponentDifficulty = Difficulty.Impossible;
                            break;
                        }
                        proponentDifficulty = proponentDifficulty.StageUp(1);
                        opponentDifficulty = opponentDifficulty.StageUp(1);
                    }
                }
                else
                {
                    Outcome = difference > 0 ? OpposedOutcomeDirection.Proponent : OpposedOutcomeDirection.Opponent;
                    Degree = (OpposedOutcomeDegree)Math.Min(Math.Abs(difference), 5);
                    ProponentDifficulty = proponentDifficulty;
                    OpponentDifficulty = opponentDifficulty;
                    break;
                }
            }
        }

        public OpposedOutcomeDirection Outcome { get; protected set; }
        public OpposedOutcomeDegree Degree { get; protected set; }
        public Difficulty ProponentDifficulty { get; protected set; }
        public Difficulty OpponentDifficulty { get; protected set; }
    }
}