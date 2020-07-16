Pack includes 36 effect prefabs.

History:
First realise: 12 effects.
Update 1.1.0:    +3 effects (15), fixed bugs.
Update 1.2.0:    optimized effect.
Update 1.3.0:    +3 effects (18), fixed bugs, optimized effects.
Update 1.4.0:    +1 effects (19).
Update 2.0.0:    +81 effects (100).
Update 2.1.0:    fixed bugs, optimized effect.
Update 2.2.0:    +36 effects (136).
Update 2.3.0:    +10 effects (146).
Update 3.0.0:    +6 effects (152), optimized effects.
Update 4.0.0:    Now in the asset 31 unique effects with the ability to change colors in one click.
Update 4.1.0:    +2 effects
Update 4.2.0:    +2 effects
Update 4.3.0:    Added support for Unity 2018.1.x+, HDRP and URP(LWRP) v1.x.x+
Update 4.3.1:    Added support for Unity 2019.3.x+, HDRP and URP(LWRP) v6.x.x+
Current update 4.4.0: Added 1 new effect for Unity 2018.1.x+

NOTE:

For correct work as in demo scene you need enable "HDR" on main camera and add CC.asset(post processing profile)
with help of Post Processing behaviour script.

Using effects:
Just drag and drop prefab of effect on scene and use that :)

Using shaders:
Slash shader works only with particle system using "Custom vertex stream.xy". Use custom data x and y. One for slash range, another for slash distance.
Tornado shader works only with particle system using "Custom vertex stream.xy". Use custom data x. This value is responsible for dissolving the effect.


If you want to use posteffect for PC like in the demo video:

1) Download unity free posteffects 
https://assetstore.unity.com/packages/essentials/post-processing-stack-83912
2) Add "PostProcessingBehaviour.cs" on main Camera.
3) Set the "PostEffects" profile. ("\Assets\ErbGameArt\Glowings orb Vol 2\Demo scene\CC.asset")
4) You should turn on "HDR" on main camera for correct posteffects. (bloom posteffect works correctly only with HDR)
If you have forward rendering path (by default in Unity), you need disable antialiasing "edit->project settings->quality->antialiasing"
or turn of "MSAA" on main camera, because HDR does not works with msaa. If you want to use HDR and MSAA then use "MSAA of post effect". 
It's faster then default MSAA and have the same quality.

Or 

1) Download Post Effects throw Package manager end enable "Bloom". Or you can use any other "Bloom".
   There are a couple of free ones at Asset Store.
   You can also create your own "Bloom" effect: https://catlikecoding.com/unity/tutorials/advanced-rendering/bloom/
2) You should turn on "HDR" on main camera for correct post-effects. (bloom post-effect works correctly only with HDR)
If you have forward rendering path (by default in Unity), you need disable antialiasing "edit->project settings->quality->antialiasing"
or turn of "MSAA" on main camera, because HDR does not works with msaa. If you want to use HDR and MSAA then use "MSAA of post effect". 
It's faster then default MSAA and have the same quality.
