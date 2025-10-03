using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

namespace TaallamGame.Dialogue
{
    public class DialogueVariables
    {
        public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

        private Story globalVariablesStory;
        private const string saveVariablesKey = "INK_VARIABLES";

        public DialogueVariables(TextAsset loadGlobalsJSON)
        {
            if (loadGlobalsJSON == null)
            {
                variables = new Dictionary<string, Ink.Runtime.Object>();
                return;
            }

            globalVariablesStory = new Story(loadGlobalsJSON.text);

            variables = new Dictionary<string, Ink.Runtime.Object>();
            foreach (string name in globalVariablesStory.variablesState)
            {
                Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
                variables.Add(name, value);
                // Removed noisy initialization logs
            }
        }

        public void SaveVariables()
        {
            if (globalVariablesStory != null)
            {
                VariablesToStory(globalVariablesStory);
                PlayerPrefs.SetString(saveVariablesKey, globalVariablesStory.state.ToJson());
            }
        }

        public void StartListening(Story story)
        {
            VariablesToStory(story);
            story.variablesState.variableChangedEvent += VariableChanged;
        }

        public void StopListening(Story story)
        {
            story.variablesState.variableChangedEvent -= VariableChanged;
        }

        private void VariableChanged(string name, Ink.Runtime.Object value)
        {
            if (variables.ContainsKey(name))
            {
                variables[name] = value;
            }
        }

        private void VariablesToStory(Story story)
        {
            foreach (KeyValuePair<string, Ink.Runtime.Object> variable in variables)
            {
                story.variablesState.SetGlobal(variable.Key, variable.Value);
            }
        }
    }
}
