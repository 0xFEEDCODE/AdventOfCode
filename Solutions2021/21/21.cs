using System.Collections.Immutable;

using Framework;

namespace Challenges2021;

public class Solution21 : SolutionFramework {
    public Solution21() : base(21) { }

    public record Player(int TotalScore, int BoardPosition, int RolledSoFarWithDiracDie = 0, DiracDie? DiracDie = null) {
        public int TotalScore = TotalScore;
        public int BoardPosition = BoardPosition;
        public int RolledSoFarWithDiracDie = RolledSoFarWithDiracDie;
        
        public void Roll(DeterministicDie deterministicDie) {
            var move = 0;
            for (var i = 0; i < 3; i++) {
                var roll = deterministicDie.RollOut();
                move += roll;
            }

            BoardPosition += move;
            if (BoardPosition % 10 == 0) {
                BoardPosition = 10;
            } else {
                BoardPosition %= 10;
            }
            TotalScore += BoardPosition;
        }

        public ICollection<Player> DiracDieRoll() {
            var pastThreeRolls = DiracDie!.RollOut();

            switch (pastThreeRolls) {
                case false:
                    return new[] {
                        this with { RolledSoFarWithDiracDie = RolledSoFarWithDiracDie + 1, DiracDie = new DiracDie(DiracDie.Roll) },
                        this with { RolledSoFarWithDiracDie = RolledSoFarWithDiracDie + 2, DiracDie = new DiracDie(DiracDie.Roll) },
                        this with { RolledSoFarWithDiracDie = RolledSoFarWithDiracDie + 3, DiracDie = new DiracDie(DiracDie.Roll) },
                    };
                case true: {
                    var newBoardPosition = BoardPosition + RolledSoFarWithDiracDie;
                    if (newBoardPosition % 10 == 0) {
                        newBoardPosition = 10;
                    } else {
                        newBoardPosition %= 10;
                    }

                    return new Player[] { new(TotalScore + newBoardPosition, newBoardPosition, 0, new DiracDie()) };
                }
            }
        }
    };

    public class Universe {
        public readonly Player Player1;
        public readonly Player Player2;
        public Universe(Player p1, Player p2) {
            Player1 = p1;
            Player2 = p2;
        }

        public int TurnOrder = 0;

        public ICollection<Universe> TakeTurn() {
            switch (TurnOrder) {
                case 0: {
                    var possibleOutcomes = Player1.DiracDieRoll();
                    //If end of roll
                    if (possibleOutcomes.Count == 1) { TurnOrder = 1; }
                    return possibleOutcomes.Select(outcome => new Universe(outcome,
                            Player2 with { DiracDie = new DiracDie(Player2.DiracDie.Roll) }) { TurnOrder = TurnOrder }).ToArray();
                }
                case 1: {
                    var possibleOutcomes = Player2.DiracDieRoll();
                    //If end of roll
                    if (possibleOutcomes.Count == 1) { TurnOrder = 0; }
                    return possibleOutcomes.Select(outcome => new Universe(
                        Player1 with { DiracDie = new DiracDie(Player1.DiracDie.Roll) }, outcome) { TurnOrder = TurnOrder }).ToArray();
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public override bool Equals(object? obj) => Equals(obj as Universe);

        public bool Equals(Universe? other) =>
            other!.Player1.TotalScore == Player1.TotalScore &&
            other.Player1.DiracDie.Roll == Player1.DiracDie.Roll &&
            other.Player1.BoardPosition == Player1.BoardPosition &&
            other.Player1.RolledSoFarWithDiracDie == Player1.RolledSoFarWithDiracDie &&
            other.Player2.TotalScore == Player2.TotalScore &&
            other.Player2.DiracDie.Roll == Player2.DiracDie.Roll &&
            other.Player2.BoardPosition == Player2.BoardPosition &&
            other.Player2.RolledSoFarWithDiracDie == Player2.RolledSoFarWithDiracDie;
        
        public override int GetHashCode() =>
            HashCode.Combine(Player1.TotalScore, Player1.DiracDie.Roll, Player1.BoardPosition, Player1.RolledSoFarWithDiracDie,
                Player2.TotalScore, Player2.DiracDie.Roll, Player2.BoardPosition, Player2.RolledSoFarWithDiracDie);
    }

    public record DiracDie(int Roll = 0) {
        public int Roll = Roll;
        
        public bool RollOut() {
            Roll++;
            var pastThreeRolls = false;
            if (Roll > 3) {
                Roll = 1;
                pastThreeRolls = true;
            }

            return pastThreeRolls;
        }
    }

    public record DeterministicDie() {
        private int roll = 0;
        public int TimesRolled { get; private set; } = 0;

        public int RollOut() {
            roll++;
            TimesRolled++;
            if (roll > 100) {
                roll = 1;
            }

            return roll;
        }
    }

    public override string[] Solve() {
        var p1 = new Player(0, 4);
        var p2 = new Player(0, 5);
        var deterministicDie = new DeterministicDie();
        while(p1.TotalScore < 1000 && p2.TotalScore < 1000){
            p1.Roll(deterministicDie);
            if (p1.TotalScore >= 1000) {
                break;
            }
            p2.Roll(deterministicDie);
        }

        var losingScore = Math.Min(p1.TotalScore, p2.TotalScore);
        AssignAnswer1(losingScore * deterministicDie.TimesRolled);

        var universes = new Dictionary<Universe, long>();
        p1 = new Player(0, 4, 0, new DiracDie());
        p2 = new Player(0, 5, 0, new DiracDie());
        universes.Add(new Universe(p1, p2), 1);
        long player1Wins = 0;
        long player2Wins = 0;
        while (universes.Values.Any()) {
            foreach (var (universe, numberOfIdenticalUniverses) in universes.ToImmutableList()) {
                universes.Remove(universe);
                var possibleUniverses = universe.TakeTurn();
                foreach (var possibleUniverse in possibleUniverses) {
                    if (possibleUniverse.Player1.TotalScore >= 21 || possibleUniverse.Player2.TotalScore >= 21) {
                        if (possibleUniverse.Player1.TotalScore > possibleUniverse.Player2.TotalScore) {
                            player1Wins+=numberOfIdenticalUniverses;
                        } else if(possibleUniverse.Player2.TotalScore > possibleUniverse.Player1.TotalScore){
                            player2Wins+=numberOfIdenticalUniverses;
                        }
                        continue;
                    }

                    if (!universes.ContainsKey(possibleUniverse)) {
                        universes.Add(possibleUniverse, 0);
                    }
                    universes[possibleUniverse]+=numberOfIdenticalUniverses;
                }
            }
        }

        AssignAnswer2(Math.Max(player1Wins, player2Wins));

        return Answers;
    }
}
