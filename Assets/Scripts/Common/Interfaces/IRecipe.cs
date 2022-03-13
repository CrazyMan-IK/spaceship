using System;
using System.Collections.Generic;

namespace Astetrio.Spaceship.Interfaces
{
    public interface IRecipe
    {
        public IReadOnlyList<IItemInformation> Componenets { get; }
        public IItemInformation Result { get; }
    }
}
