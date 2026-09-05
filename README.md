UITK_SimpleTimeline is essentially a recreation of the built-in Animation window, but using a custom UI Elements interface instead. 
It doesn't edit AnimationClip assets, but instead a custom struct called: SimpleTimeline. 
These SimpleTimelines contain a float duration, a bool to decide if it should loop, and a list of TimelineCurves which
hold all of the keyframe information. 
The TimelineCurve class itself is abstract, so the user must make their own versions of it (inheriting from TypedTimelineCurve<,> so
that it can actually be used for something).
