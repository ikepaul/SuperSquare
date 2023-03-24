using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSquare
{
    internal class Stage
    {
        public int Id;
        public float StartTime { get; set; }
        public Action StageModifiers { get; set; }
        public Stage(int id, float startTime, Action stageModifiers) { 
            Id = id;
            StartTime = startTime;
            StageModifiers = stageModifiers;
        }
    }
}
