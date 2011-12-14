using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XNATools.UI
{
    public class SkillTree : UIControl
    {
        struct Skill
        {
            public string Name { get; set; }
            public SkillButton Button { get; set; }

            public Skill(string name, SkillButton button)
                : this()
            {
                Name = name;
                Button = button;
            }
        }

        private Dictionary<int, List<Skill>> tree;
        public int TierAdvanceCost { get; set; }
        public int PointsSpent { get; private set; }
        public int MaxPoints { get; set; }

        public SkillButton this[string name]
        {
            
        }

        public SkillTree(Game game, Vector2 position)
            : base(game, position)
        {
            tree = new Dictionary<int, List<Skill>>();
            TierAdvanceCost = 4;
        }

        public void AddSkill(string name, SkillButton button, int tier)
        {
            if (!tree.ContainsKey(tier))
            {
                tree[tier] = new List<Skill>();
                Bounds = new Rectangle(Bounds.X, Bounds.Y, 0, tree.Keys.Max() * (64 + 10));
            }
            button.StacksChanged += (o, e) => UpdateButtons();
            tree[tier].Add(new Skill(name, button));
            UpdateButtons();
            PositionButtons();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (List<Skill> tier in tree.Values)
            {
                foreach (Skill skill in tier)
                {
                    if (skill.Button.Enabled)
                    {
                        skill.Button.Update(gameTime);
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (List<Skill> tier in tree.Values)
            {
                foreach (Skill skill in tier)
                {
                    skill.Button.Draw(gameTime);
                }
            }

            base.Draw(gameTime);
        }

        private void PositionButtons()
        {
            foreach (var tier in tree)
            {
                for (int i = 0; i < tier.Value.Count; i++)
                {
                    tree[tier.Key][i].Button.Position = new Vector2(Position.X + i * (64 + 10), Bounds.Bottom - tier.Key * (64 + 10));
                }
            }
        }

        private void UpdateButtons()
        {
            PointsSpent = 0;

            foreach (var tier in tree)
            {
                foreach (var skill in tier.Value)
                {
                    PointsSpent += skill.Button.Stacks;
                }
            }

            int topTierWithPoints = 0;
            int pointsSpentInTopTier = 0;
            if (PointsSpent > 0)
            {
                topTierWithPoints = tree.Where(t => t.Value.Sum(s => s.Button.Stacks) > 0).Max(tier => tier.Key);
                pointsSpentInTopTier = tree[topTierWithPoints].Sum(t => t.Button.Stacks);
            }

            foreach (var tier in tree)
            {
                foreach (var skill in tier.Value)
                {
                    if (tier.Key == topTierWithPoints)
                    {
                        skill.Button.AllowDecrease = true;
                    }
                    else
                    {
                        skill.Button.AllowDecrease = PointsSpent > topTierWithPoints * TierAdvanceCost + pointsSpentInTopTier;
                    }

                    skill.Button.AllowIncrease = PointsSpent < MaxPoints;
                    
                    skill.Button.Enabled = PointsSpent >= tier.Key * TierAdvanceCost;
                    skill.Button.Enabled = !(skill.Button.Stacks < 1 && PointsSpent >= MaxPoints);
                }
            }
        }
    }
}
