using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;
using Astetrio.Spaceship.Interfaces;

namespace Astetrio.Spaceship.Models
{
    [CreateAssetMenu(fileName = "New ChansedRandomItemInformation", menuName = "Astetrio/Spaceship/Chansed Random Item Information", order = 62)]
    public class ChansedRandomItemInformation : ScriptableObject, IItemInformationPresenter
    {
        [SerializeField] private List<ItemInformation> _items = new List<ItemInformation>();
        [SerializeField] private List<float> _probability = new List<float>();

        public IItemInformation GetInformation()
        {
            return SelectRandom(_items, _probability);
        }

        private static T SelectRandom<T>(List<T> items, List<float> probability)
        {
            if (items.Count < 1 || items.Count != probability.Count)
            {
                return default;
            }

            var min = probability.Min();
            min = Mathf.Max(1 / min, 1);
            var convertedProbability = Scan(probability, (acc, x) =>
            {
                return acc + x;
            }, 0f).Skip(1);//.Select((x, i) => (x, i)).OrderBy(x => x.x);

            var random = URandom.Range(0, (int)(100 * min));

            int i = 0;
            foreach (var prob in convertedProbability)
            {
                if (random < prob * min)
                {
                    return items[i];
                }

                i++;
            }

            throw new InvalidProgramException();
        }

        private static IEnumerable<U> Scan<T, U>(IEnumerable<T> input, Func<U, T, U> next, U state)
        {
            yield return state;
            foreach (var item in input)
            {
                state = next(state, item);
                yield return state;
            }
        }
    }
}
