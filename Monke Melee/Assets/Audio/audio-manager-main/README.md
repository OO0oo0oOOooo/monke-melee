# Basic audio manager

# Features
* Custom PlayRandomClipAtPosition() uses AudioMixer
* PlayOnParent() creates audiosource on a transform that loops until StopOnParent()

# Setup
* Drop prefab in scene
* Assign the AudioSources from children
* Create Audio Clips scriptable object
* Assign Scriptable objects
* Assign Mixer Group that you want PlayRandomClipAtPosition() and PlayOnParent() to use
* Place VolumeSettings script in the scene and assign the mixer and ui sliders
