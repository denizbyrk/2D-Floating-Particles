# Floating Particles

A 2D particle physics simulator coded in C# and uses Monogame Framework for graphics rendering.

## Demo

<img src="https://github.com/user-attachments/assets/d0d793ff-d817-4597-a7d2-b2f0d53e14ad" alt="Loading..." width="500">

## Installation

The following are the instructions for running the code:

- Download the project as a ZIP file or clone it using GitHub Desktop.
- Open the project in Visual Studio or VS Code.
- Make sure you have C# installed.
  - If you are using Visual Studio, Install .NET desktop development.
  - If you are using VS Code, run the following command:
    
  ```
  code --install-extension ms-dotnettools.csharp
  ```
  
  - To verify installation run the following command:
    
  ```
  dotnet --version
  ```
  
- Install Monogame
  - If you are using Visual Studio you can install Monogame through Extensions window.
  - If you are using VS Code, you can run the following command to install Monogame Templates:
    
  ```
  dotnet new --install MonoGame.Templates.CSharp
  ```

- Run the code

## How to Use

The project has no keyboard input, it only uses Mouse left click.

From the button menu, you can change the settings and colors.

<img src="https://github.com/user-attachments/assets/a4b14461-b008-46e8-9f75-40043dd6ec65" alt="Loading..." width="500">

You can manage:

- Particle Speed
- Particle Size
- Spawned Particles
- Frequency (Frames between particle spawns)
- Radius (How far the particles will spawn from the mouse)
- Random Spawn
- Edge Collision
- Decay and Decay Speed
- Friction and Friction Value
- Pausing Simulation
- Colors

## Code

Here is the brief explanation for what the files are responsible for.  

The explanation of the codes are written as comment lines in the code itself.

- **Main.cs:** The code starts up from here. From here you can manage the screen size, running speed, controls and other management controls of the project.

- **Particle.cs:** Class for managing particle properties.

- **Util.cs:** Class for taking mouse and keyboard inputs, and helper methods.

- **Button.cs:** Class for buttons.
