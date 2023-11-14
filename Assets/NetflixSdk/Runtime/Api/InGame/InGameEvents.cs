using System;
using System.ComponentModel;

namespace Netflix
{
    // https://stash.corp.netflix.com/pages/CL/cl-types/master/browse/docs/classes/cl.game.ingame.newgamestart.html
    [Serializable]
    public class NewGameStart : InGameEvent
    {
        public NewGameStart() : base("game.inGame.NewGameStart")
        {
            // no-op
        }
    }
    
    // https://stash.corp.netflix.com/pages/CL/cl-types/master/browse/docs/classes/cl.game.ingame.ftuestart.html
    [Serializable]
    public class FirstTimeUserExperienceStart : InGameEvent
    {
        public FirstTimeUserExperienceStart() : base("game.inGame.FTUEStart")
        {
            // no-op
        }
    }
    
    // https://stash.corp.netflix.com/pages/CL/cl-types/master/browse/docs/classes/cl.game.ingame.ftuecomplete.html
    [Serializable]
    public class FirstTimeUserExperienceComplete : InGameEvent
    {
        public FirstTimeUserExperienceComplete() : base("game.inGame.FTUEComplete")
        {
            // no-op
        }
    }
    
    // https://stash.corp.netflix.com/pages/CL/cl-types/master/browse/docs/classes/cl.game.ingame.ftuestepcomplete.html
    [Serializable]
    public class FirstTimeUserExperienceStepComplete : InGameEvent
    {
        public double stepNumber { get; }
        public string stepName { get; }
        [DefaultValue("")]
        public string stepDesc { get; set; }
        public FirstTimeUserExperienceStepComplete(double stepNumber, string stepName, string stepDesc = "") 
            : base("game.inGame.FTUEStepComplete")
        {
            this.stepNumber = stepNumber;
            this.stepName = stepName;
            this.stepDesc = stepDesc;
        }
    }
    
    // https://stash.corp.netflix.com/pages/CL/cl-types/master/browse/docs/classes/cl.game.ingame.progresscheckpointcomplete.html
    [Serializable]
    public class ProgressCheckpointComplete : InGameEvent
    {
        public double checkpointNumber { get; }
        public string checkpointName { get; }
        public string checkpointType { get; }
        [DefaultValue("")]
        public string checkpointDesc { get; set; }

        public ProgressCheckpointComplete(double checkpointNumber, string checkpointName, string checkpointType,
            string checkpointDesc = "")
            : base("game.inGame.ProgressCheckpointComplete")
        {
            this.checkpointNumber = checkpointNumber;
            this.checkpointName = checkpointName;
            this.checkpointType = checkpointType;
            this.checkpointDesc = checkpointDesc;
        }
    }
    
    // https://stash.corp.netflix.com/pages/CL/cl-types/master/browse/docs/classes/cl.game.ingame.gamecomplete.html
    [Serializable]
    public class GameComplete : InGameEvent
    {
        public GameComplete() : base("game.inGame.GameComplete")
        {
            // no-op    
        }
    }
    
    [Serializable]
    public class Custom : InGameEvent
    {
        public string json;
        
        public Custom(string name, string json) : base(name)
        {
            this.json = json;
        }

        public override string ToJson()
        {
            return json;
        }
    }
}