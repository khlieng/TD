using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TD
{
    public class ToggleGroup
    {
        private List<IToggleAble> toggleAbles = new List<IToggleAble>();

        public ToggleGroup()
        {
        }

        public ToggleGroup(IEnumerable<IToggleAble> toggleAbles)
        {
            foreach (IToggleAble t in toggleAbles)
            {
                Add(t);
            }
        }

        public ToggleGroup(params IToggleAble[] toggleAbles)
        {
            foreach (IToggleAble t in toggleAbles)
            {
                Add(t);
            }
        }

        public ToggleGroup Add(IToggleAble toggleAble)
        {
            toggleAbles.Add(toggleAble);

            toggleAble.ToggledChanged += (o, e) =>
                {
                    if (toggleAble.Toggled)
                    {
                        foreach (IToggleAble t in toggleAbles)
                        {
                            if (t != toggleAble)
                            {
                                t.Toggled = false;
                            }
                        }
                    }
                };

            return this;
        }

        public void Add(IEnumerable<IToggleAble> toggleAbles)
        {
            foreach (IToggleAble t in toggleAbles)
            {
                Add(t);
            }
        }

        public void Add(params IToggleAble[] toggleAbles)
        {
            foreach (IToggleAble t in toggleAbles)
            {
                Add(t);
            }
        }

        public void UnToggleAll()
        {
            foreach (IToggleAble t in toggleAbles)
            {
                t.Toggled = false;
            }
        }
    }
}
