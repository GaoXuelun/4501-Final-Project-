# 4501Project
Yibo Sun, 101082357

ZhenFeng, 101172075

Yuhan Wang, 101165518



# Operation
Barrack.cs, BuidlingPlacement.cs, Barracks.prefab, BarrackButton, BaseButton:

Open project, folder Scenes -> folder Map1, open scene Map1

Run game, can use mouse to move screen, left mouse buttom down to draw a rectangle selection area, up buttom to finish select. Selected units will have a selected circle.

For selected units, right mouse buttom down to move. If mouse on enemy, cursor will change, and if click enemy, characters will attack. With attack, enemy hp bar will decrease (for test, only works on TrainingDummy).

Click Base or Barrack button on left-bottom corner to select building, then left click a position on map to build or right click to cancel.

Mouse on barrack, cursor will change, and click barrack to generate footman.


# Operation (new)
Hermite.cs

Open project, folder Scenes -> folder Hermite Spline, open scene Hermite Spline:

Attach Hermite.cs script to an object. In the inspector, set waypoints in Waypoint Objects, and set tangents of waypoints in Tangents. The number of waypoints and tangents should be the same (There already made a sample scene, can just click start to run the game and see the demo).


Open project, folder Scenes -> folder Map1, open scene Map1:

Made a new map to replace the previous draft proposal, the new map is much larger with more (map) details.

Improved the animation part, animator of player units and part enemies are done. They will have different effects depending on different states. Such as idle/walk/run, base attack/critical attack/get hit(only critical attack)/die.

UI and animation can better interact with player behavior now, select units to fight with enemies (for attacks, once the mouse clicks on the target, units will attack once), and they will enter the corresponding animation state, and the hp bar will correctly drop with health loss. Once the hp bar drop 0, units will die.

There are 2 teams on the map for testing, one of the player units (hero), and one for the enemy (wizard).

For further details about animators, can see the Animations folder.

Changing data to test combat, can change the character/attack data in the Character folder (or find by scripts on unit prefab).

Improved the unit movement so that units can follow another unit now. Select units a, right-click another player unit b, then player control that unit b, a will follow b. If there is any other action before selecting b will break follow command.

There are some obstacles around player units on the map to test obstacle avoidance behavior, units can well pass them and hold formation at the target point.


# Files
Animations > Animator > Characters > Footman >
                                   > TinyHero >
                                   > FemaleCharacter, Footman, Hero, MaleCharacter
                      > Enemy > Wizard > 
                              > Grunt
                              > Wizard
Materials >
Mesh >
Plugins (Assets from package manager) >
Prefabs (Build based on Plugins or self model) > UI >
                                               > Units >
                                               > etc.
Scenes > Hermite Spline >
       > Map1 (project scene) >
       > Test (use to Test) >       
Scripts > Building 
        > Character > Character Information > MonoBehavior | ScriptableObject
                    > Controller >
                    > Data >
        > Controller >
        > Hermite Spline >
        > UI >
Texture (Selected circle) >
Settings (Lighting, Pipeline,.etc) >


# Method reference
https://www.youtube.com/watch?v=pJQndtJ2rk0&t=1369s

https://www.youtube.com/watch?v=BLfNP4Sc_iA

https://www.bilibili.com/video/BV1X84y1F7aY/?spm_id_from=333.999.0.0

https://www.bilibili.com/video/BV1Za411g7sf/?vd_source=4d1f41de674667d535107fe3d8b045f9

https://www.youtube.com/watch?v=mvPRub-T2WM

https://www.bilibili.com/video/BV1ka41177mD/?spm_id_from=333.337.top_right_bar_window_history.content.click

# Resources from Assets Store
# cursor:
https://assetstore.unity.com/packages/2d/textures-materials/basic-rpg-cursors-139404

# character:
https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/rpg-hero-pbr-hp-polyart-121480
https://assetstore.unity.com/packages/3d/characters/humanoids/rpg-tiny-hero-duo-pbr-polyart-225148
https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/mini-legion-footman-pbr-hp-polyart-86576

https://assetstore.unity.com/packages/3d/props/lowpoly-training-dummy-202311

https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/battle-wizard-poly-art-128097
https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/mini-legion-lich-pbr-hp-polyart-91497

https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/mini-legion-rock-golem-pbr-hp-polyart-94707
https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/mini-legion-grunt-pbr-hp-polyart-98187

# skybox:
https://assetstore.unity.com/packages/vfx/shaders/free-skybox-extended-shader-107400

# Environment and Building
https://assetstore.unity.com/packages/3d/props/stylized-crystal-77275
https://assetstore.unity.com/packages/3d/props/stylized-fantasy-props-sample-234139
https://assetstore.unity.com/packages/3d/environments/fantasy/ree-slavic-medieval-environment-town-interior-and-exterior-167010
https://assetstore.unity.com/packages/2d/textures-materials/texture-stone-square-181918
https://assetstore.unity.com/packages/3d/props/exterior/low-poly-resource-rocks-76150
https://assetstore.unity.com/packages/2d/textures-materials/stylized-lava-materials-180943
https://assetstore.unity.com/packages/3d/props/toon-crystals-pack-66182
https://assetstore.unity.com/packages/3d/environments/free-viking-pack-192579