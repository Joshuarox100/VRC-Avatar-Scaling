VRC Avatar Scaling
==============
>Due to the recent update in the beta, the current release of this doesn't work. This page will be updated when a working revision is completed.

Author: Joshuarox100

Usage: This package is meant for use with VRChat's Avatars 3.0 (VRCSDK-AVATAR)

Description: Scale your avatar to any size on the fly with a spin of the joystick!

Contains:
- Two Animations
- One Animator
- One Expressions Menu
- This README

Setting Up:
--------------
Before following these steps, set up your avatar how you normally would and ensure that you have a basic understanding of how Avatars 3.0 works.

1) Right click the object holding your Avatar Descriptor and create an empty object named "Scaler" (without the quotation marks).
	>This is needed for the animations that actually scale your avatar.

<p align="center">
  <img width="40%" height="40%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/Step%201.png">
</p>

2) Add a Scale Constraint component to your Armature, assign "Scaler" as a source, and click Activate (Don't click Zero).
	>This will cause your Armature (and any other objects using this object in a Scale Constraint component) to multiply their original scale with the scale of the Scale Constraint source object (in this case, "Scaler"). You can test this by simply changing the scale of the "Scaler" object.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/Step%202.png">
</p>

3) In the Avatar Descriptor, assign the included Animator to your Action layer.
	>If you already have a custom Action layer, you can copy the layers and parameters from the included Animator into your own. Keep in mind that if you don't copy the parameters before the copying the states, the conditions for transitioning between the states may be modified, so be sure to correct them or things will behave strangely (or not at all).

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/Step%203.png">
</p>

4) If you don't already have one, create a VRCStageParameters asset in your project (Create -> VRC Scriptable Objects -> Stage Parameters).

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/Step%204.png">
</p>

5) Add the following three variables anywhere within your Stage Parameters list (without the quotation marks):
	1. "MenuState" (Int): Used for telling the Animator which menu you're in.
	2. "Scale" (Float): Used for setting the scale of your avatar.
	3. "Reset" (Int): Used for resetting the scale and the viewpoint.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/Step%205.png">
</p>

6) Add your Stage Parameters asset to the included Expressions Menu.
	>If the parameters and values for the menu controls aren't automatically set, have the Reset control set "Reset" to 1 and have the Size control set "MenuState" to 4 and use "Scale" for Parameter Rotation.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/Step%206.png">
</p>

7) In the Avatar Descriptor, either use the included Expressions Menu or use it as a submenu within your own.

<p align="center">
  <img width="80%" height="80%" src="https://github.com/Joshuarox100/VRC-Avatar-Scaling/blob/Tutorial-Images/Tutorial%20Images/Step%207.png">
</p>

Everything should now be fully set up! If you have any issues or questions, look in the troubleshooting and questions section below before contacting me.

Common Questions:
--------------
**Does this also scale the IPD when you change size?**
>Unfortunately no. Currently, Animator Set View does not have an option to recalculate the IPD Scale after setting the view, however I have made a feedback post on the VRChat Canny for adding this feature that you can upvote here: [[Feedback] Option to Adjust the IPD Scale for Animator Set View](https://bit.ly/2ZY0SdG)

**What is IPD and why does it matter for this?**
>IPD stands for interpupillary distance or, in layman's terms, the distance between the center of your eyes. In VRChat, IPD Scale is calculated using the height of your viewpoint and is used to set how large you and your play space are relative to the world (among other things). You can see exactly how it effects an avatar by enabling or disabling the IPD Scale within the Avatar Descriptor and testing it in VRChat. The scale used for an avatar is only calculated while loading the avatar and there is currently no way to recalculate it using a different height afterwards (as of the writing of this README). If it could be adjusted via Animator Set View (The script used to move the viewpoint), the scale of your play space could be made to adjust alongside the scale of the avatar. This would eliminate the current situation of the avatar changing size while you experience an instantaneous elevator ride to the head bone. You can upvote the feedback post I made on the VRChat Canny linked in the question above if you want to help in showing the devs a need for this feature.

**How do I change the minimum and maximum size I can go between?**
>Simply adjust the scale values set in the included Animations named "Min" and "Max" to whatever you'd like (make sure to change the values for both frames of the Animation you're modifying).

**How do I change the animation used when adjusting the scale?**
>By default, your avatar's Idle animation is used while adjusting your scale. If you want to have a different animation other than your Idle play, put the Animation you want on both the "Unlocking View" and "Waiting" states within the "Scale (View)" layer of your Animator.

**Can I remove the slight delay between setting a new size and having the viewpoint move?**
>Yes, however this can cause your viewpoint to not be set if you swap between large sizes too quickly as further explained in the troubleshooting section. If you still want to remove it, set the transition exit time from the "Unlocking View" state to the "Waiting" state to however long you want within the "Scale (View)" layer of the Animator.

**Why does "MenuState" get set to 4 and not 1?**
>The avatar I made this for uses "MenuState" for triggering other Animator states by opening or closing a menu as well. It just so happened that the fourth menu I made was the one used for scaling the avatar. If you want to change this value because of OCD go right ahead, just don't forget to change the conditions within the Animator as well when making your changes.

Troubleshooting:
--------------
**Avatar is super large or small after activating the Scale Constraint component.**
>You did what I said **not** to do in Step 2, didn't you? If you did listen and this still happens, try repeating Steps 1 and 2 and ensure that the "Scaler" object is within the root of your Avatar next to Armature and Body. Additionally, make sure that you **don't** hit the Zero button on the Scale Constraint component at any time as it will set your Armature's scale to 1, messing up any models that scaled up their Armatures when exporting.

**Viewpoint starts to drift higher or lower from its intended location the more you scale.**
>Animator Set View currently doesn't account for changing the scale of the head bone. It instead retains its position relative to the position of your head bone. I've made another post on the VRChat Canny explaining this in more detail here: [[Bug] Animator Set View does not account for changing the scale of the head bone.](https://bit.ly/3gXIPex)

**Viewpoint doesn't appear to move when using Full Body Tracking (FBT).**
>Unfortunately, Animator Set View doesn't seem to be working correctly with FBT at the moment. Even if it did, it would look really weird since it doesn't currently adjust IPD Scale which would cause your legs to float in the air and make you look like a ball (refer to 'What is IPD' in Common Questions).

**Viewpoint doesn't move to where you want it to go.**
>The place your viewpoint moves to depends on where your head bone is located after setting your scale. By default, this package uses VRChat's T-Pose animation and takes the head position as where your view should go. This works fine usually, but if you are using a model that doesn't work correctly with VRChat's T-Pose animation, you'll need to create your own animation to use instead. Regardless of how you decide to create your animation, keep in mind that the viewpoint will be set to the location of the avatar's head bone within it. Once you have an animation you're happy with, replace the proxy_tpose Animation used in the "Updating View" state contained within the "Scale (View)" layer of the Animator.

**IPD doesn't scale (T-Rex arms / Room floor above or below world floor).**
>There is currently no option for Animator Set View to recalculate the IPD Scale after moving the viewpoint (refer to 'What is IPD' in Common Questions), however I have made a feedback post on the VRChat Canny about adding this feature that you can upvote here: [[Feedback] Option to Adjust the IPD Scale for Animator Set View](https://bit.ly/2ZY0SdG)

**Viewpoint doesn't get set when switching between sizes quickly.**
>This should only happen if you adjusted the values for the included "Min" and "Max" Animations or changed the exit time for resetting the viewpoint. You can fix this by increasing the transition exit time between the "Unlocking View" and "Waiting" states within the "Scale (View)" layer of the Animator. This happens because of the linear amount of time it takes to reset the viewpoint from whatever height you set it to. While the viewpoint is resetting, it isn't possible to set a new one until it's finished.

**Can't crouch or prone after setting the viewpoint (Desktop).**
>*I think* this is a bug with Animator Set View regarding Desktop mode. You may be able to get around this by creating some extra states for checking if you're crouched or in prone, but you're on your own there.

**People see you at your minimum size when resetting.**
>Not exactly sure why this happens. I think it has something to do with the way parameters are synced currently, but you can fix it by holding down the included Reset control for about a second.

**VRChat's "Avatar Reset" button doesn't reset the viewpoint (Desktop).**
>This seems to be a bug with using Desktop mode currently. You can use the Reset control within the included VRC Expressions Menu to reset it correctly. Additionally, I've made a post about this bug on the VRChat Canny you can upvote here: [[Bug] Animator Set View isn't reset after using the default reset button in Desktop](https://bit.ly/2C51eaa)

**VRChat's "Avatar Reset" button doesn't reset the viewpoint (VR).**
>The Avatar Reset button only works for uploaded Avatars, so it won't work if you're just using Build and Test. If your Avatar is uploaded and it's still not working, you can always use the Reset control within the included VRC Expressions Menu to reset it correctly.

Contacting Me:
--------------
If you still have some questions or recommendations you'd like to throw my way, you can ask me on Discord (Joshuarox100#5024) or leave a suggestion or issue on the [GitHub](https://github.com/Joshuarox100/VRC-Avatar-Scaling) page.
