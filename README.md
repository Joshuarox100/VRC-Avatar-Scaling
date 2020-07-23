VRC Avatar Scaling
==============

Author: Joshuarox100

Usage: This package is meant for use with VRChat's Avatars 3.0 (VRCSDK3-AVATAR)

Description: Scale your avatar to any size on the fly with a spin of the joystick!

Contains:
- One Animation
- Two Animators
- One Avatar Mask
- One Expressions Menu
- This README

Setting Up
--------------
Before following these steps, set up your avatar how you normally would and ensure that you have a basic understanding of how Avatars 3.0 works. If you'd rather follow a video guide for setting this up, go [here](Video URL).

0) Download and import the latest **Unity Package** from [**Releases**](https://github.com/Joshuarox100/VRC-Avatar-Scaling/releases) on GitHub **(You will have issues if you don't)**.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/0-4/Step%200.png">
</p>

1) In the Avatar Descriptor, assign the "Scaling" Animator to your Gesture layer.
	>If you use Full-Body Tracking, also assign the "TPose" Animator to your TPose layer (this corrects your scale for others when you press Calibrate). Additionally, if you already have a custom Gesture layer, you can copy the states and parameters from the included Animator into your own [as shown here](Video URL).

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/0-4/Step%201.png">
</p>

2) If you don't already have one, create a VRCStageParameters asset in your project (Create -> VRC Scriptable Objects -> Stage Parameters).

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/0-4/Step%202.png">
</p>

3) Add the following two variables anywhere within your Stage Parameters list (without the quotation marks):
	1. "Scale" (Float): Used for setting the scale of your avatar.
	2. "SizeOp" (Int): Used for knowing when to update the viewpoint.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/0-4/Step%203.png">
</p>

4) Add your Stage Parameters asset to the included Expressions Menu.
	>If the parameters and values for the menu controls aren't automatically set, have the Reset control set "SizeOp" to 2 and have the Size control set "SizeOp" to 1 and use "Scale" for Parameter Rotation.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/0-4/Step%204.png">
</p>

5) In the Avatar Descriptor, either use the included Expressions Menu or use it as a submenu within your own.
	>If you need to adjust the minimum, maximum, or default scale, adjust the values contained within the "Size Settings" Animation [as seen here](Video URL).

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/0-4/Step%205.png">
</p>

Everything should now be fully set up! If you have any issues or questions, look in the [troubleshooting](#troubleshooting) and [questions](#common-questions) section below before [contacting me](#contacting-me).

Common Questions
--------------
**Does this also scale the IPD when you change size?**
>Yes! As of a recent update to the VRChat Avatars 3.0 beta, there is now a way to remeasure the height of the avatar and adjust the IPD Scale accordingly.

**How do I change the animation used when adjusting the scale?**
>By default, your avatar's Idle animation is used while adjusting your scale. If you want to have a different animation other than your Idle play, you'll need to create a layer in the (preferably Action) Animator that plays the animation while "SizeOp" is 1.

Troubleshooting
--------------
**Avatar is smaller or larger than normal when loading in or adjusting scale.**
>This can happen if the default scale of your avatar isn't 1. Fix this by changing the values within the "Size Settings" Animation to better fit your avatar as shown in the linked video for Step 5 of the tutorial. This can also be caused if you merged the "Scaling" layer into another Animator and didn't set the "Scale" parameter to 0.5.

**Unable to open the "Size" menu.**
>If you're using "Scale Menu" as a submenu, ensure sure that selecting it in the previous menu doesn't set SizeOp's value.

**The menu doesn't change size when you do.**
>VRChat has already stated that this will be fixed in the next version of the beta. It will update the size when you open one of the main menus like the one for Avatars or Worlds.

**Avatar starts walking weirdly after using Avatar Reset.**
>I don't know why this happens honestly, but it's a known issue that only seems to occur when using local testing. It shouldn't happen for uploaded avatars.

**Viewpoint and Full Body trackers start to drift when changing size (FBT).**
>This seems to currently be an issue with Animator Remeasure Avatar that I don't have control over. A Canny post about this bug has been made by bd_ that you can upvote here: [[BUG] FBT calibration seems to drift when scaling an avatar using Remeasure Avatar](https://feedback.vrchat.com/avatar-30/p/bug-fbt-calibration-seems-to-drift-when-scaling-an-avatar-using-remeasure-avatar)

**Feet either move farther apart or closer together after changing size (Non-FBT).**
>This is actually a problem with using Auto Footsteps. It currently retains the distance between the feet at your starting scale even after using Animator Remeasure Avatar. I've made a Canny post about it that you can upvote here: [[Bug] Auto Footsteps doesn't update correctly after using Remeasure Avatar.](https://feedback.vrchat.com/avatar-30/p/bug-auto-footsteps-doesnt-update-correctly-after-using-remeasure-avatar)

Contacting Me
--------------
If you still have some questions or recommendations you'd like to throw my way, you can ask me on Discord (Joshuarox100#5024) or leave a suggestion or issue on the [GitHub](https://github.com/Joshuarox100/VRC-Avatar-Scaling) page.