KlakLASP
========

**KlakLASP** is an extension for [Klak] to create audio reactive behaviors with
using the Klak Wiring system.

![Screenshot](http://i.imgur.com/jAtUMDvl.png)

KlakLASP integrates the [LASP] plugin to analyze audio signals. Thanks to the
low-latency nature of LASP, it doesn't introduce noticeable delays between
audio input and actions.

![GIF](http://i.imgur.com/NoG5EtG.gif)
[Demo](http://radiumsoftware.tumblr.com/post/163292853756)

System Requirements
-------------------

- Unity 2017.1 or later

At the moment, KlakLASP only supports Windows (64 bit) and macOS (64 bit).

Installation
------------

Before installing KlakLASP to a project, all its dependent plugins ([Klak] and
[LASP]) should be installed to the project. Follow the installation
instructions in each page.

Then, download one of the unitypackage files from the [Releases] page and
import it to the project.

How It Works
------------

The Audio Input node has a VU meter in the inspector, and it shows how the node
analyzes input audio signals and determines output.

![GIF](http://i.imgur.com/e4QyD1u.gif)

The node tracks the recent peak amplitude of the input audio signals, and
determine the output based on the difference between the current amplitude and
the peak amplitude.

When the current amplitude is equal or larger than the peak amplitude, it
outputs a value of 1. The VU meter indicates over-peak input with a red band
(see the image below).

![Meter](http://i.imgur.com/6iIJB8z.png)

When the current amplitude is smaller than the peak amplitude and larger than
the value of *(peak amplitude - dynamic range)*, it outputs a value between 0
and 1 that varies based on the position in the dynamic range. This range is
indicated with a gray band in the VU meter (see the image below).

![Meter](http://i.imgur.com/ecLQf0t.png)

When the current amplitude is equal or smaller than the value of *(peak
amplitude - dynamic range)*, it outputs a value of 0.

![Meter](http://i.imgur.com/tDPkH7X.png)

The peak amplitude value slowly decreases by time to adapt itself to the recent
amplitude level. The GIF above shows this behavior that the peak amplitude and
the dynamic range are slightly slid to the left while the input doesn't hit the
peak.

Note that the peak amplitude doesn't exactly match with the actual peak of the
input signals -- there is a small margin between them, so that it increases the
chance of hitting the peak.

Audio Input Properties
----------------------

### Filter Type

Four types of filters are available -- *Bypass*, *Low-Pass*, *Band-Pass* and
*High-Pass*. These filter are useful to detect rhythmic accents. For instance,
the *Low-Pass* filter can be used to make a behavior that reacts to kick drums
or a bassline.

### Dynamic Range (dB)

Specifies the difference between the lowest amplitude and the highest
amplitude in decibel.

### Auto Gain Control

When enabled, it automatically adjust the amount of the gain based on recent
peak of amplitude (as explained in the *How It Works* section). When disabled,
the peak amplitude is fixed to 0dB, and the amount of the gain is manually
controlled by the *Gain* property.

### Hold And Fall Down

Enables the "peak-hold and fall down" behavior that is commonly used in VU
meters. This is useful to make animation smoother. The GIF below shows the
difference between with/without this option.

![GIF](http://i.imgur.com/dhxqaH3.gif)

TIPS
----

### Dynamic Range = Clickiness of Behavior

Although having a wide dynamic range is important for expressiveness, it tends
to make behavior slower and unclear. It's recommended reducing the dynamic
range when "clicky" behavior is preferred.

![GIF](http://i.imgur.com/ljVUjxV.gif)

### Use internal audio sources

**[LASP Loopback]** can be used as a substitute of the LASP plugin. It analyzes
audio output from Unity instead of external audio sources. This is useful to
create audio reactive behaviors with internal audio sources.

[LASP Loopback]: https://github.com/keijiro/Lasp/tree/loopback

Current Limitations
-------------------

- LASP always tries to use the system default device for recording. There is no
  way to use a device that is not assigned as default.
- LASP only supports monophonic input. Only the first channel (the left channel
  in case of stereo input) will be enabled when using a multi-channel audio
  device.

License
-------

[MIT](LICENSE.txt)

[Klak]: https://github.com/keijiro/Klak
[LASP]: https://github.com/keijiro/Lasp
[Releases]: https://github.com/keijiro/KlakLasp/releases
