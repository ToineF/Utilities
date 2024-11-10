using System;
using System.Linq;

namespace DialogueSystem
{
    public class DialogueEffectsFactory
    {
        public DialogueEffect CreateDialogueEffect(string type, string value, int index)
        {
            /*Type[] types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where assemblyType.IsSubclassOf(typeof(DialogueEffect))
                select assemblyType).ToArray();

            foreach (Type d in types)
            {
               // if (type.Name == dialogueEffect.Type) 
            }*/

            if (type == "S") return new SpeedChangeEffect(type, value, index);
            else if (type == "W") return new WaitEffect(type, value, index);

            return new DialogueEffect(type, value, index);
        }
    }
}