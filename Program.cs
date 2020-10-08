using System;
using System.Collections.Generic;
using Abilities;

namespace GameEventHandlerExample
{
    public class Program
    {
        public HashSet<Unit> units = new HashSet<Unit>();
        public static void Main(string[] args)
        {
            Program p = new Program();
            p.Game();
        }

        public void Game()
        {
            GameEventHandler.UnitDied += HandleUnitDeath;
            int newAvengers = 0;
            var im = new HashSet<DamageType> { DamageType.Fire };
            Unit originalAvenger = Unit.CreateUnit(this, "Bob the OG Avenger", 10, null, 2, new HashSet<Ability> { new Abilities.Avenger(2) });
            var defendingUnit = Unit.CreateUnit(this, "Example", 10, im, 5, new HashSet<Ability> { new Abilities.DeathAnouncer() });
            Console.WriteLine("Press 'x' to do fire damage, 'y' to do energy damage, 'z' to do dark damage");
            foreach (Unit u in units)
            {
                Console.WriteLine(u);
            }
            char keyCode = 'a';
            while (keyCode != 'q')
            {
                keyCode = Console.ReadKey(true).KeyChar;
                switch (keyCode)
                {
                    case 'x':
                        defendingUnit.DealDamage(DamageType.Fire, 1);
                        break;
                    case 'y':
                        defendingUnit.DealDamage(DamageType.Energy, 1);
                        break;
                    case 'z':
                        defendingUnit.DealDamage(DamageType.Darkness, 1);
                        break;
                    case 'b':
                        newAvengers++;
                        Unit u = Unit.CreateUnit(this, $"Dave {newAvengers} the Avenger", 10, null, 1, new HashSet<Ability> { new Abilities.Avenger(1) });
                        break;
                    case 'c':
                        var shielder = Unit.CreateUnit(this, "Shielder", 2, null, 0, new HashSet<Ability> { new Abilities.Shielder() });
                        break;
                    default:
                        break;
                }
                foreach (Unit u in units)
                {
                    Console.WriteLine(u);
                }
                Console.WriteLine();
            }
        }

        public void HandleUnitDeath(object sender, OnUnitDiedEventArgs e)
        {
            e.DeadUnit.AbilityList.Clear();
            units.Remove(e.DeadUnit);
        }

    }
}