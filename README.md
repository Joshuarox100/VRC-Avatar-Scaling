VRC Avatar Scaling
==============

Author: Joshuarox100

Usage: This package is meant for use with VRChat's Avatars 3.0 (VRCSDK3-AVATAR)

Description: Scale your avatar to any size on the fly with a spin of the joystick!

Contains:
- Three Animations
- One Animator
- One Expressions Menu
- This README

Setting Up:
--------------
Before following these steps, set up your avatar how you normally would and ensure that you have a basic understanding of how Avatars 3.0 works.

1) In the Avatar Descriptor, assign the included Animator to your Action layer.
	>If you already have a custom Action layer, you can copy the layers and parameters from the included Animator into your own. Keep in mind that if you don't copy the parameters before the copying the states, the conditions for transitioning between the states may be modified, so be sure to correct them or things will behave strangely (or not at all).

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/Step%203.png">
</p>

2) If you don't already have one, create a VRCStageParameters asset in your project (Create -> VRC Scriptable Objects -> Stage Parameters).

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/Step%204.png">
</p>

3) Add the following three variables anywhere within your Stage Parameters list (without the quotation marks):
	1. "MenuState" (Int): Used for telling the Animator which menu you're in.
	2. "Scale" (Float): Used for setting the scale of your avatar.
	3. "Reset" (Int): Used for resetting the scale and the viewpoint.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/Step%205.png">
</p>

4) Add your Stage Parameters asset to the included Expressions Menu.
	>If the parameters and values for the menu controls aren't automatically set, have the Reset control set "Reset" to 1 and have the Size control set "MenuState" to 4 and use "Scale" for Parameter Rotation.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/Step%206.png">
</p>

5) In the Avatar Descriptor, either use the included Expressions Menu or use it as a submenu within your own.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/Step%207.png">
</p>

Everything should now be fully set up! If you have any issues or questions, look in the troubleshooting and questions section below before contacting me.

Common Questions:
--------------
**Does this also scale the IPD when you change size?**
>Yes! As of a recent update to the VRChat Avatars 3.0 beta, there is now a way to remeasure the height of the avatar and adjust the IPD Scale accordingly.

**How do I change the minimum and maximum size I can go between?**
>Simply adjust the scale values set in the included Animations named "Min" and "Max" to whatever you'd like (make sure to change the values for both frames of the Animation you're modifying).

**How do I change the animation used when adjusting the scale?**
>By default, your avatar's Idle animation is used while adjusting your scale. If you want to have a different animation other than your Idle play, put the Animation you want on the "Waiting" state within the "Scale (View)" layer of your Animator.

**Can I remove the slight delay between setting a new size and having the viewpoint move?**
>No, the component responsible for remeasuring your height takes about half a second to work. Unless if VRChat changes this, it will remain unchangeable.

**Why does "MenuState" get set to 4 and not 1?**
>The avatar I made this for uses "MenuState" for triggering other Animator states by opening or closing a menu as well. It just so happened that the fourth menu I made was the one used for scaling the avatar. If you want to change this value because of OCD go right ahead, just don't forget to change the conditions within the Animator as well when making your changes.

**Why does the Action layer need to always stay at 1?**
>For whatever reason, there is a bug with VRChat regarding Animations that modify the avatar root. If an Animator on an avatar contains an Animation that modifies the scale of the avatar root in any way, the avatar's scale when loading in will be doubled (or set to 2, not sure), even if the Animation isn't being played. To combat this, the "Default" Animation is constantly played when you aren't scaled so your scale is set correctly when loading in. Technically it doesn't need to be done in the Action layer, but I wanted to simplify the installation for others so I didn't use another Animation layer.

Troubleshooting:
--------------
**Avatar is extremely small or large when loading in.**
>This will happen if the default scale of your avatar isn't 1. Fix this by simply changing the values within the "Default" Animation to the default scale of your avatar.

**Avatar is extremely small or large when adjusting scale.**
>This will happen if the default scale of your avatar isn't 1 and you haven't adjusted the "Min" and "Max" Animations. Fix this by adjusting the "Min" and "Max" Animations to better fit your avatar.

**People see you at your minimum size when resetting.**
>Currently, there is a delay between updating a parameter and having it sync to other clients. By default, the Animator gives 1 second for syncronizing the Reset button. If this happens to you frequently, increase the exit time for the transition between "Resetting: Network" and "Resetting: Local" in the "Scale (Size)" layer of your Animator (60 = 1 second).

**The menu doesn't change size when you do.**
>VRChat has already stated that this will be fixed in the next version of the beta. It will update the size when you open one of the main menus like the one for Avatars or Worlds.

Contacting Me:
--------------
If you still have some questions or recommendations you'd like to throw my way, you can ask me on Discord (Joshuarox100#5024) or leave a suggestion or issue on the [GitHub](https://github.com/Joshuarox100/VRC-Avatar-Scaling) page.
