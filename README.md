VRC Avatar Scaling
==============

Author: Joshuarox100

Usage: This package is meant for use with VRChat's Avatars 3.0 (VRCSDK3-AVATAR)

Description: Scale your avatar to any size on the fly with a spin of the joystick!

Contains:
- Four Animations
- One Animator
- One Animator Override Controller
- One Expressions Menu
- This README

Setting Up:
--------------
Before following these steps, set up your avatar how you normally would and ensure that you have a basic understanding of how Avatars 3.0 works.

0) Download the latest release's **Unity Package** from the [**Releases**](https://github.com/Joshuarox100/VRC-Avatar-Scaling/releases) section on GitHub **(You will have issues if you don't)**.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%200.png">
</p>

1) In the Avatar Descriptor, assign the included Animator Override Controller ("Scaling Override") to your Action layer.
<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%201.png">
</p>

>If you already have a custom Action layer, you can copy the layers and parameters from the included Animator into your own using these steps **(Also be sure to read the answer for *"Why does the Action layer need to always stay at 1?"* in the [Common Questions](#common-questions) section below)**.
	‏‏‎ ‎  
	<p align="center">Copy the parameters from the included Animator into your own.<br>
	  <img width="75%" height="75%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%20A.png">
	</p>
	<p align="center">Create two new layers within your own Animator<br>(I recommend you name them the same so you can identify them later).<br>
	  <img width="75%" height="75%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%20B.png">
	</p>
	<p align="center">Copy the states from each layer in the included Animator into<br>each of their respective layers that you created within your own Animator.<br>
	  <img width="75%" height="75%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%20C.png">
	</p>
	<p align="center">In each layer, set the default state to "Ready" if it isn't already.<br>
	  <img width="75%" height="75%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%20D.png">
	</p>
	<p align="center">Set the weight of both layers to 1.00 within your Animator.<br>
	  <img width="75%" height="75%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%20E.png">
	</p>
	<p align="center"><b>(Optional)</b><br>Create a new Animator Override Controller to use with your Animator<br>and use it for your Action layer in the Avatar Descriptor.<br>
	  <img width="75%" height="75%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%20F.png">
	</p>

2) If you don't already have one, create a VRCStageParameters asset in your project (Create -> VRC Scriptable Objects -> Stage Parameters).

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/New/Step%202.png">
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

Everything should now be fully set up! If you have any issues or questions, look in the troubleshooting and questions section below before contacting me.

Common Questions:
--------------
**Does this also scale the IPD when you change size?**
>Yes! As of a recent update to the VRChat Avatars 3.0 beta, there is now a way to remeasure the height of the avatar and adjust the IPD Scale accordingly.

**How do I change the minimum and maximum size I can go between?**
>Make a copy of the included "Min" and "Max" Animations, adjust them how you like, and use them in the "Scaling Override" (make sure to change the values for both frames of the Animation).

**How do I change the animation used when adjusting the scale?**
>By default, your avatar's Idle animation is used while adjusting your scale. If you want to have a different animation other than your Idle play, put the Animation you want to play instead in the "Custom" slot of the "Scaling Override".

**Why does "MenuState" get set to 4 and not 1?**
>The avatar I made this for uses "MenuState" for triggering other Animator states by opening or closing a menu as well. It just so happened that the fourth menu I made was the one used for scaling the avatar. If you want to change this value because of OCD go right ahead, just don't forget to change the conditions within the Animator as well when making your changes.

**Why does the Action layer need to always stay at 1?**
>For whatever reason, there is a bug with VRChat regarding Animations that modify the avatar root. If an Animator on an avatar contains an Animation that modifies the scale of the avatar root in any way, the avatar's scale when loading in will be doubled (or set to 2, not sure), even if the Animation isn't being played. To combat this, the "Default" Animation is constantly played when you aren't scaled so your scale is set correctly when loading in. Technically it doesn't need to be done in the Action layer, but I wanted to simplify the installation for others so I didn't use another Animation layer. I've posted about this bug on the VRChat Canny in a post that you can view and upvote here: [[Bug] The presence of Animations affecting the root avatar scale within an Animator cause the avatar to be the incorrect size.](https://feedback.vrchat.com/avatar-30/p/bug-the-presence-of-animations-affecting-the-root-avatar-scale-within-an-animato)

Troubleshooting:
--------------
**Avatar is smaller or larger than normal when loading in.**
>This will happen if the default scale of your avatar isn't 1. Fix this by making a copy of the "Default" Animation, changing the values within it to the default scale of your avatar, and using it in the "Scaling Override" (make sure to change the values for both frames of the Animation).

**Avatar is smaller or larger than expected when adjusting scale.**
>This will happen if the default scale of your avatar isn't 1 and you aren't using custom "Min" and "Max" Animations. Fix this by making a copy of the included "Min" and "Max" Animations, adjust them to better fit your avatar, and use them in the "Scaling Override" (make sure to change the values for both frames of the Animation).

**The menu doesn't change size when you do.**
>VRChat has already stated that this will be fixed in the next version of the beta. It will update the size when you open one of the main menus like the one for Avatars or Worlds.

**Viewpoint and Full Body trackers start to drift when changing size (FBT).**
>This seems to currently be an issue with Animator Remeasure Avatar that I don't have control over. A Canny post about this bug has been made by bd_ that you can upvote here: [[BUG] FBT calibration seems to drift when scaling an avatar using Remeasure Avatar](https://feedback.vrchat.com/avatar-30/p/bug-fbt-calibration-seems-to-drift-when-scaling-an-avatar-using-remeasure-avatar)

Contacting Me:
--------------
If you still have some questions or recommendations you'd like to throw my way, you can ask me on Discord (Joshuarox100#5024) or leave a suggestion or issue on the [GitHub](https://github.com/Joshuarox100/VRC-Avatar-Scaling) page.