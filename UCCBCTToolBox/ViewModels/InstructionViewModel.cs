using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCCBCTToolBox.ViewModels
{
    internal class InstructionViewModel
    {
        public string Cephalo { get; set; }
        public InstructionViewModel()
        {
            Cephalo = "Position CBCT into a frontal fov. Clicking cephalo will drop a grid that can be resized over the parietal bones. Use the center dot to move the graph and the corners to resize. +/- keys will expand or shrink the graph. </> will rotate the graph. And the arrow keys will translate the graph. When the tilt of the cephalo fits the head tilt of the patient, enter key will finalize and show the angle of head tilt. Autoload button will copy that into the head tilt section of the main calcs. Double check L/R on head tilt manually.";
        }
    }
}
