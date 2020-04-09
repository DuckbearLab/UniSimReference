INTRODUCTION
------------

Thank you for purchasing this package! Check out modmonkeys.net for all the other stuff I made.

If you have any questions or issues feel free to contact me:
- In the official forum thread of the package:
  http://forum.unity3d.com/threads/172763-Time-of-Day-Realistic-day-night-cycle-and-atmospheric-scattering
- Via personal message on the Unity forums:
  http://forum.unity3d.com/members/30479-plzdiekthxbye
- On my website:
  http://modmonkeys.net/contact



USING THE PACKAGE
-----------------

First of all, you should decide whether you want to work in linear or gamma space:
  * The "Sky Dome" prefab has a checkbox called "Linear Lighting"
  * This should only be checked if linear lighting is enabled for your project

  REMARK
  Linear lighting is a Unity Pro feature that can be enabled under "Edit->Project Settings->Player->Color Space"
  For more information on linear lighting visit http://docs.unity3d.com/Documentation/Manual/LinearLighting.html

To add the sky dome to your scene:
  * Drag the "Sky Dome" prefab into your scene
  * Set the parameters of the "Sky.cs" script component to your liking

To fit the sky dome size to your clipping plane:
  * Adjust the scale of your sky dome instance according to the clipping plane set in your camera
  * The scale in x, y and z direction should be equal

To move the sky dome to the camera position in every frame:
  * Add the script "SkyCamera.cs" to your camera
  * Drag your instance of the sky dome onto the the "Sky" property of the "SkyCamera.cs" script

  REMARK
  This script moves the sky dome directly before clipping the scene, guaranteeing that all other position updates have
  been processed. You should not move the sky dome in "LateUpdate" because this can cause minor differences in the sky
  dome position between frames when moving the camera.

To enable the dynamic day / night cycle:
  * The sky dome prefab has a script "SkyTime.cs" attached to it, but it is disabled by default
  * Enable this component by checking the according checkbox
  * Set the parameters of this script to your liking

To switch between pre-defined weather types:
  * The sky dome prefab has a script "SkyWeather.cs" attached to it, but it is disabled by default
  * Enable this component by checking the according checkbox
  * Set the parameters of this script to your liking
  * Adding your own weather types is very easy as the script is essentially just one enum and a switch statement

To use a custom skybox at night:
  * Disable the child object called "Space"
  * Setup your camera to render the skybox of your choice, it will automatically be visible at night



DESKTOP VS MOBILE
-----------------

Differences between the "Sky Dome" prefab and the "Sky Dome (Mobile)" prefab:
  * Reduced sky dome mesh vertex count on mobile
  * The parameters "Cloud Sharpness" and "Cloud Shading" are being ignored on mobile

  REMARK
  The reason for this is that "Cloud Sharpness" and "Cloud Shading" have to be applied per-pixel and are therefore
  very costly on mobile devices. However, you can achieve the same effect as the "Cloud Sharpness" parameter by
  manually editing the cloud noise texture. The package comes with three textures for three kinds of cloud coverage,
  all you have to do is set the noise texture in the cloud shader appropriately.

Depending on your project, newer mobile devices (iPhone 4S / iPad 2 or better) might be capable of rendering the
desktop version of this package. However, as there are almost no visual downgrades in the mobile version you might
be better off investing any free computation time you have into other components of your scene.



RENDERING ORDER
---------------

All components of the sky dome are being rendered after the opaque but before the transparent meshes of your scene.
That means that only areas of the sky dome that are not being occluded by any other geometry have to be rendered.

The rendering order of the sky dome components is the following:
  1. Space
  2. Moon
  3. Atmosphere
  4. Sun
  5. Clouds

This usually leads to 3-5 draw calls to render the complete sky dome. If you want to save additional draw calls
on mobile devices you can for example remove the halo material from the moon renderer.



SKY DOME PARAMETERS
-------------------

# Color Space
Linear Lighting: Should be consistent with your project's color space setting (true if linear)

# Cycle
Time Of Day: The time of the day in hours
Julian Date: The date of the year in days
Latitude:    The latitude of your position
Longitude:   The longitude of your position

# Day
Rayleigh Multiplier: The intensity of atmospheric Rayleigh scattering
Mie Multiplier:      Tthe intensity of atmospheric Mie scattering
Brightness:          The brightness of the sky color derived by the physical model
Haziness:            The intensity of the haziness (i.e. fogginess) of the sky at day
Color:               The base color of the sky at day

# Night
Haziness:    The intensity of the haziness (i.e. fogginess) of the sky at night
Color:       The base color of the sky at night
Haze Color:  The haze color at night
Cloud Color: The cloud color at night

# Sun
Light Intensity:  The intensity of the sun light source
Falloff:          Controls how fast the sun color will darken
                  This is especially visible towards the begin / end of the day
Coloring:         Controls how strongly the sun color affects the overall sky color
                  This is especially visible towards the begin / end of the day

# Moon
Light Intensity: The intensity of the moon light source
Phase:           The phase of the moon
                 - 0: full moon
                 - ±1: no moon

# Clouds
Tone:      The tone multiplier of the cloud layer
           - 0: black clouds
           - otherwise: brighter clouds
Shading:   The shading multiplier of the cloud layer
           - 0: constant color over the entire cloud layer
           - otherwise: shading is being multiplied to the cloud color
Density:   The density multiplier of the cloud layer
           - 0: no clouds
           - otherwise: thicker clouds
Sharpness: The sharpness multiplier of the cloud layer
           - 0: one giant cloud
           - otherwise: several smaller clouds
Speed 1:   The movement speed of the first cloud layer
Speed 2:   The movement speed of the second cloud layer
Scale 1:   The texture scale of the first cloud layer
Scale 2:   The texture scale of the second cloud layer



LITERATURE
----------

The following literature has been used to implement physically correct atmospheric scattering:

[1] http://www.cs.utah.edu/~shirley/papers/sunsky/sunsky.pdf
[2] http://www2.imm.dtu.dk/pubdb/views/publication_details.php?id=2554
[3] http://developer.amd.com/wordpress/media/2012/10/GDC_02_HoffmanPreetham.pdf
[4] http://www.ati.com/developer/dx9/ATI-LightScattering.pdf
[5] http://www.vis.uni-stuttgart.de/~falkmn/pubs/wscg07-schafhitzel.pdf

These papers are being referenced in the code in the following way:

"See [N] page P equation (E)"

N: Paper #
P: Page #
E: Equation # (if available)
