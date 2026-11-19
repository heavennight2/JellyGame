PixelFX Parallax Worlds - Free Sample
Pixel art parallax backgrounds with seamless horizontal layers.
Made with Parallax Generator.

WHAT'S INSIDE
- 12 backgrounds (3 scenes) in these sizes: 320x180 px.
- Every background has its own folder with: layers/ (one PNG per layer, tileable horizontally), composite.png (all layers together), a scrolling GIF that loops (in some folders), parallax_scene.tscn (Godot 4.3+, Parallax2D), parallax.json and README.txt.
- Variants: many scenes also come as dawn, dusk and night (and spring, autumn and winter for the forest).

SCENES
  - Forest by day
  - Under the sea
  - Heart world

FOLDERS
  scene/WIDTHxHEIGHT/variant/   (for example: forest_by_day/320x180/night/)

HOW TO USE
Godot 4.3+: copy the pack folder to your project (keep the res:// path used when it was created: res://parallax/), open a parallax_scene.tscn and add it to your level. Each layer is a Parallax2D with its scroll scale already set; with a Camera2D the layers move at different speeds. Use texture filter Nearest.
Other engines: draw the layers of layers/ from back to front (file names are numbered), scroll each one horizontally at (scroll_scale x camera speed, see parallax.json) and repeat it every image width.

FULL PACK
This is a free sample (3 scenes). The full PixelFX Parallax Worlds pack has 20 scenes and 100 backgrounds in two sizes:
https://akisez.itch.io/pixelfx-parallax-worlds

LICENSE
See LICENSE.txt.
