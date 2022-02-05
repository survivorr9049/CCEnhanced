# CCEnhanced
Enhancements for Unity's build in Character Controller 
Features:
- smooth step, allowing player for seamless movement on ledges, and irregular terrain, it's not recommended for stairs as regular ramps are smoother
- alternative ground detection with margin and radius
- detection of obstacles above player's head
- shrinking player's height from top to bottom, as opposed to vanilla character controller behaviour of shrinking to the center
# Usage
add CCEnhanced script instead of Character Controller to your player,
enable Sync Transform in Physics settings (required for smooth step to work properly)

I commit changes here directly from sorsemovement reloaded, so example controller might not work properly during development, it will be fixed when i am sure that basic features
are stable and work properly.
