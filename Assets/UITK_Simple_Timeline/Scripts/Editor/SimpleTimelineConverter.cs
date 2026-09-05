#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor.UIElements;
namespace UITK_SimpleTimeline.Editor
{

    public class SimpleTimelineConverter : UxmlAttributeConverter<SimpleTimeline> //not sure if i even need this?
    {
        static string ValueToString(object obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);

        public override SimpleTimeline FromString(string value)
        {
            SimpleTimeline s = new();
            object[] objects = value.Split('|');
            s.Loop = (bool)Convert.ChangeType(objects[0], typeof(bool), CultureInfo.InvariantCulture);
            s.Duration = (float)Convert.ChangeType(objects[1], typeof(float), CultureInfo.InvariantCulture);
            s.Curves = (List<TimelineCurve>)Convert.ChangeType(objects[2], 
                typeof(List<TimelineCurve>), 
                CultureInfo.InvariantCulture);
            return s;
        }

        public override string ToString(SimpleTimeline s)
        {
            string format = "";
            format += ValueToString(s.Loop + "|");
            format += ValueToString(s.Duration + "|");
            format += ValueToString(s.Curves);
            return format;
        }
    }
}
#endif