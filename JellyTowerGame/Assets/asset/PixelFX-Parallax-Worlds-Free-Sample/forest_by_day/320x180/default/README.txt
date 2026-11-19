Parallax background (320 x 180 px)
Made with Parallax Generator.

FILES
- layers/*.png: one image per layer. They tile horizontally with no seams.
- composite.png: all layers together.
- parallax_preview.gif: scrolling preview that loops.
- parallax_scene.tscn: Godot 4 scene using Parallax2D (copy the 'layers' folder to the res:// path used when exporting).
- parallax.json: size and speed of every layer.

LAYERS (back to front)
- 00_sky.png  (scroll scale 0.0)
- 01_clouds.png  (scroll scale 0.167)
- 02_mountains.png  (scroll scale 0.167)
- 03_hills.png  (scroll scale 0.333)
- 04_forest.png  (scroll scale 0.5)
- 05_forest.png  (scroll scale 0.833)
- 06_ground.png  (scroll scale 1.0)

HOW TO USE
Godot 4.3+: open parallax_scene.tscn and add it to your level. Each layer is a Parallax2D with the right scroll_scale; with a Camera2D the layers move at different speeds. Use texture filter Nearest.
Other engines: draw the layers back to front, scroll each one horizontally at (layer scale x camera speed) and repeat it every 320 px.
