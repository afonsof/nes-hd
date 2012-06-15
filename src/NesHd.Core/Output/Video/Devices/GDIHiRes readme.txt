MyNes GDI HiRes Video Mode Readme

Date: 2010-8-25
Writen By: mkwong98 

Introduction
============
Inspired by the Glide64 graphics plugin for N64 emulators and Resident Evil games on PC where the user can replace textures in the game with their own graphics pack to take advantage of the higher resolution of the PC platform, this video mode allows the users to replace graphics in a game with their own by creating their of graphics pack. 

The screen of a NES game consists of tiles which are 8x8 pixels in size and each can have up to 3 colours excluding the transparent pixels. The user can replace them with tiles of 16x16 pixels in 24 bits colour. However this will not work with games where the graphics are computed in real time.

How to create graphics pack
===========================
For better understanding please refer to the example pack (BombSweeper, the rom can be found at http://www.zophar.net/).

1. Graphics pack structure
All the files in the pack are stored inside a folder with the same name as the game rom and is located in the rom folder. The pack consists of image files and a text file named "hires.txt" which defines when the image files are used. 

2. Image file format
The images are stored in png format and only pixels with alpha = 0 are treated as transparent pixel and any other value will be treated as opaque.

3. Format of hires.txt
The file contains lines of text and each line can either set an image to be loaded or define when a tile is to be replaced. To load an image, start the line with "<img>" followed by the file name. To define a tile replacement, start the line with "<tile>" followed by nine pieces of information separated with comma. See the next section for a complete explanation. Lines starting with any other pattern will be ignored.

4. Replacement definition
The nine pieces of information are as follow:
i.   The CHR page number where the tile is stored.
ii.  The tile offset within the CHR page.
iii. The index of the image to be used. The index of first image loaded is 0, the second image is 1, the third image is 2 and so on.
iv.  The first colour ID in palette used by the tile. On the NES, one tile can use many different palettes during the game. However doing the same with 24 bit colour will be very difficult. This video mode solves the problem by letting the user to use different images when the palette has changed. And in order to do so, we must tell the emulator which palette we are working on.
v.   The second colour ID in the palette.
vi.  The third colour ID in the palette.
vii. The x coordinate of the top left corner of the replacement tile inside the image.
viii.The y coordinate of the top left corner of the replacement tile inside the image.
ix.  Whether this tile replacement can be used as the default replacement when no palette can be matched, "Y" = default. If no default replacement is defined, the tile will be rendered using the normal method.

To find out these details, make a snapshot and the video mode will also create a log file listing the CHR page number, tile offset, x coordinate of the tile, y coordinate of the tile and the three colour IDs in the palette for each tile displayed on the screen.

Tips
====
1. Have the first image to be a blank image as in the example pack. This will come in handy in many situations.
2. This mode get slower when more pixels are replaced. So don't start with the background tiles unless you are slowing down the game on purpose.

FAQ
===
Q: Why?
A: The NES is my first console and many of my favourite games are on the NES but the graphics do not age well unlike SNES. So it will be a big boost to the NES emulators if the graphics can be updated.

Q: What about music and sound?
A: I'm not very good at music and know nothing about sound processing so it is beyound my capability. I hope my effort will inspire someone to start a project which replace the backgroud music with updated performance.

Q: Why MyNes?
A: I looked at the code of several open source emulators and this is the only one that I can understand. :P

Q: Why GDI?
A: I don't know much about SlimDX and GDI is easy to get into. Hope someone will port it to SlimDX or other emulators and make it faster.

Q: Future development?
A: Currently the only ones that I can think of is the support of semi-transparent pixels and a fast mode that render at 8x8 instead of 16x16. But now I'm more interested in creating graphics packs than adding more features. 

Special Thanks
==============
Nintendo for making this console.
Ala Hadid, AHD. for making MyNes.
Microsoft for making the software used.
