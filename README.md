VRC Avatar Scaling
==============

Author: Joshuarox100

Usage: This package is meant for use with VRChat's Avatars 3.0 (VRCSDK3-AVATAR)

Description: Scale your avatar to any size on the fly with a spin of the joystick!

Setting Up
--------------
Before following these steps, set up your avatar how you normally would and ensure that you have a basic understanding of how Avatars 3.0 works.

1) Download and import the latest **Unity Package** from [**Releases**](https://github.com/Joshuarox100/VRC-Avatar-Scaling/releases) on GitHub **(You will have issues if you don't)**.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/0-5/0/Step%201.png">
</p>

2) Open the Avatar Scaling window located under Window -> Avatar Scaling.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/0-5/0/Step%202.png">
</p>

3) Next, configure how you want scaling to be setup for your avatar:
	>If you're not sure what a particular setting does, hover your mouse over the text to see its function or refer to [Setup Window](#setup-window).

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/0-5/0/Setup%20Window.png">
</p>

4) Finally, click the Apply button to automatically set up scaling for your avatar! 
	>Data will only ever be overwritten in the following circumstances:  
		1. A file already exists with the same name as one being generated.  
		2. An Animator already contains a layer named 'Scaling' (the layer is replaced).  
		3. An Animator already contains a parameter of the correct name and type (the value is overwritten).

Everything should now be fully set up! If you have any issues or questions, look in the [troubleshooting](#troubleshooting) and [questions](#common-questions) section below before [contacting me](#contacting-me).

Setup Window
--------------
<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/0-5/0/Setup%20Window.png">
</p>

| Setting | Function |
| --------- | ---------- |
| Active Avatar | The Avatar you want to setup scaling for. |
| Expressions Menu | (Optional) The Expressions Menu you want the scaling controls added to.<br>Leave this empty if you don't want any menus to be affected.<br>(Controls will be added as a submenu.) |
| Add Parameters | (Optional) Check for needed parameters within the Avatar's Expression Parameters and add them if necessary. |
| Use Existing Animators | (Optional) If Animators are already present for Gesture, Sitting, or TPose, the parameters and layer for scaling will be added to them.<br>If an Animator is missing or this feature is disabled, a new Animator will be generated using the defaults included in the VRChat SDK and inserted into the descriptor automatically. |
| Multiplier | (On) Use sizes relative to your starting scale.<br>(Off) Use the exact values you will scale to. |
| Minimum | The minimum scale your avatar can be. |
| Maximum | The maximum scale your avatar can be. |
| Curve Type | What curve the scaling Animation should use. |
| Path | The folder where any generated files will be saved to.<br>(Generated files will overwrite existing files with the same name: \<AVATAR NAME>_\<TEMPLATE NAME>) |

Common Questions
--------------
**Does this also scale the IPD when you change size?**
>Yes!

**How do I change the animation used when adjusting the scale?**
>By default, any other Animations that are currently playing will continue to play while you change your scale. If you want to make a specific Animation play, use the Action layer to play it when 'SizeOp' is equal to 1.

**How will I look to those who have my Animations disabled?**
>If someone has custom animations turned off, you'll appear to either be floating or partially in the floor at your default size. Your IK will still work correctly though, so don't worry about your arms flying into space or crushing your internal organs when you change size, they won't do that.

Troubleshooting
--------------
**Avatar sinks into the floor upon loading in.**
>If this begins happening after an update to the VRCSDK occured, delete your TPose Animator and try the setup again with your previous settings and it should fix itself.

**Unable to open the 'Scale' menu.**
>Verify that your menu doesn't set 'SizeOp' or 'Scale' upon opening the submenu.

**The menu doesn't change size when you do.**
>VRChat has already stated that this will be fixed *soon*™. It will update the size when you open one of the main menus like the one for Avatars or Worlds.

**Avatar starts walking weirdly after using Reset Avatar.**
>This is a known issue with VRChat that only seems to occur when using local testing. It shouldn't happen on uploaded avatars.

**Avatar is stuck T-Posing either remotely or locally.**
>Uncertain of the cause, but the only way to fix it is by leaving and rejoining whatever world you're in.

**Viewpoint and Full Body trackers start to drift when changing size (FBT).**
>This seems to currently be an issue with Animator Remeasure Avatar that I don't have control over. A Canny post about this bug has been made by bd_ that you can upvote here: [[BUG] FBT calibration seems to drift when scaling an avatar using Remeasure Avatar](https://feedback.vrchat.com/avatar-30/p/bug-fbt-calibration-seems-to-drift-when-scaling-an-avatar-using-remeasure-avatar)

**Feet either move farther apart or closer together after changing size (Non-FBT).**
>This is actually a problem with using Auto Footsteps. It currently retains the distance between the feet at your starting scale even after using Animator Remeasure Avatar. I've made a Canny post about it that you can upvote here: [[Bug] Auto Footsteps doesn't update correctly after using Remeasure Avatar.](https://feedback.vrchat.com/avatar-30/p/bug-auto-footsteps-doesnt-update-correctly-after-using-remeasure-avatar)

**Avatar Scaling window refuses to open.**
>To fix this, reset your Editor layout by clicking Layout in the top right of the Editor, clicking Reset Factory Settings, and attempt to open the window again.

**"An exception occured!"**
>If this happens, ensure you have a clean install of Avatar Scaling, and if the problem persists, [let me know](#contacting-me)!

Contacting Me
--------------
If you still have some questions or recommendations you'd like to throw my way, you can ask me on Discord (Joshuarox100#5024) or leave a suggestion or issue on the [GitHub](https://github.com/Joshuarox100/VRC-Avatar-Scaling) page.