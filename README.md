VRC Avatar Scaling
==============

Author: Joshuarox100

Usage: This package is meant for use with VRChat's Avatars 3.0 (VRCSDK3-AVATAR)

Description: Scale your avatar to any size on the fly with a spin of the joystick!

Contains:
- Three Animations
- One Animator
- One Avatar Mask
- One Expressions Menu
- This README

Setting Up
--------------
Before following these steps, set up your avatar how you normally would and ensure that you have a basic understanding of how Avatars 3.0 works. If you'd rather follow a video guide for setting this up, look [here]().

0) Download and import the latest **Unity Package** from [**Releases**](https://github.com/Joshuarox100/VRC-Avatar-Scaling/releases) on GitHub **(You will have issues if you don't)**.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%200.png">
</p>

1) In the Avatar Descriptor, assign the included Animator to your Gesture layer.
<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%201.png">
</p>

>If you already have a custom Gesture layer, you can copy the states and parameters from the included Animator into your own using these steps.
	‏‏‎ ‎  
	<p align="center">Copy the parameters from the included Animator into your own.<br>
	  <img width="75%" height="75%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%20A.png">
	</p>
	<p align="center">Create a new layer within your own Animator<br>(I recommend you name it Scale or Scaling so you can identify it later on).<br>
	  <img width="75%" height="75%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%20B.png">
	</p>
	<p align="center">Select and copy all the states from the "Scale" layer in the<br>included Animator into the new layer that you created within your own Animator.<br>
	  <img width="75%" height="75%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%20C.png">
	</p>
	<p align="center">In the new layer, set the "Ready" state as the default layer state if it isn't already.<br>
	  <img width="75%" height="75%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%20D.png">
	</p>
	<p align="center">Set the weight of the new layer to 1.00 within your Animator.<br>Also set the layer's mask to the included Avatar Mask, "Non-Body".<br>
	  <img width="75%" height="75%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%20E.png">
	</p>

2) If you don't already have one, create a VRCStageParameters asset in your project (Create -> VRC Scriptable Objects -> Stage Parameters).

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/Step%202.png">
</p>

3) Add the following three variables anywhere within your Stage Parameters list (without the quotation marks):
	1. "MenuState" (Int): Used for telling the Animator which menu you're in.
	2. "Scale" (Float): Used for setting the scale of your avatar.
	3. "Reset" (Int): Used for resetting the scale and the viewpoint.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%203.png">
</p>

4) Add your Stage Parameters asset to the included Expressions Menu.
	>If the parameters and values for the menu controls aren't automatically set, have the Reset control set "Reset" to 1 and have the Size control set "MenuState" to 4 and use "Scale" for Parameter Rotation.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%204.png">
</p>

5) In the Avatar Descriptor, either use the included Expressions Menu or use it as a submenu within your own.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%205.png">
</p>

Everything should now be fully set up! If you have any issues or questions, look in the [troubleshooting](#troubleshooting) and [questions](#common-questions) section below before [contacting me](#contacting-me).

Common Questions
--------------
**Does this also scale the IPD when you change size?**
>Yes! As of a recent update to the VRChat Avatars 3.0 beta, there is now a way to remeasure the height of the avatar and adjust the IPD Scale accordingly.

**How do I change the minimum and maximum size I can go between?**
>Simply adjust the scale values set in the included Animations named "Min" and "Max" to whatever you'd like (make sure to change the values for both frames of the Animation you're modifying).

**How do I change the animation used when adjusting the scale?**
>By default, your avatar's Idle animation is used while adjusting your scale. If you want to have a different animation other than your Idle play, you'll need to create a layer in the (preferably Action) Animator that plays the animation while "MenuState" is 4.

**Why does "MenuState" get set to 4 and not 1?**
>The avatar I made this for uses "MenuState" for triggering other Animator states by opening or closing a menu as well. It just so happened that the fourth menu I made was the one used for scaling the avatar. If you want to change this value because of OCD go right ahead, just don't forget to change the conditions within the Animator as well when making your changes.

Troubleshooting
--------------
**Avatar is smaller or larger than normal when loading in.**
>This will happen if the default scale of your avatar isn't 1. Fix this by simply changing the values within the "Default" Animation to the default scale of your avatar (make sure to change the values for both frames of the Animation).

**Avatar is smaller or larger than expected when adjusting scale.**
>This will happen if the default scale of your avatar isn't 1 and you haven't adjusted the "Min" and "Max" Animations. Fix this by adjusting the "Min" and "Max" Animations to better fit your avatar (make sure to change the values for both frames of the Animation you're modifying).

**Unable to open the "Size" menu.**
>If you're using "Scale Menu" as a submenu, ensure sure that selecting it in the previous menu doesn't set MenuState's value.

**The menu doesn't change size when you do.**
>VRChat has already stated that this will be fixed in the next version of the beta. It will update the size when you open one of the main menus like the one for Avatars or Worlds.

**Avatar starts walking weirdly after using Avatar Reset.**
>I don't know why this happens honestly, but it only seems to happen occasionally when using local testing. It shouldn't occur on uploaded avatars.

**Viewpoint and Full Body trackers start to drift when changing size (FBT).**
>This seems to currently be an issue with Animator Remeasure Avatar that I don't have control over. A Canny post about this bug has been made by bd_ that you can upvote here: [[BUG] FBT calibration seems to drift when scaling an avatar using Remeasure Avatar](https://feedback.vrchat.com/avatar-30/p/bug-fbt-calibration-seems-to-drift-when-scaling-an-avatar-using-remeasure-avatar)

**Feet either move farther apart or closer together after changing size (Non-FBT).**
>This is actually a problem with using Auto Footsteps. It currently retains the distance between the feet at your starting scale even after using Animator Remeasure Avatar. I've made a Canny post about it that you can upvote here: [[Bug] Auto Footsteps doesn't update correctly after using Remeasure Avatar.](https://feedback.vrchat.com/avatar-30/p/bug-auto-footsteps-doesnt-update-correctly-after-using-remeasure-avatar)

Contacting Me
--------------
If you still have some questions or recommendations you'd like to throw my way, you can ask me on Discord (Joshuarox100#5024) or leave a suggestion or issue on the [GitHub](https://github.com/Joshuarox100/VRC-Avatar-Scaling) page.