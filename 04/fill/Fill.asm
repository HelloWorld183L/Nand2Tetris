// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input.
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel;
// the screen should remain fully black as long as the key is pressed. 
// When no key is pressed, the program clears the screen, i.e. writes
// "white" in every pixel;
// the screen should remain fully clear as long as no key is pressed.

  @status
  M=-1 // -1="black" or 0xFFFF
  D=0  // Argument - what to set screen bits to
  @SETSCREEN
  0;JMP

  (LOOP)
    @KBD
    D=M   // D = current keyboard character
    @SETSCREEN
    D;JEQ // If no key is pressed, set screen to zeroes (white)
    D=-1  // If a key is pressed, set screen to all 1 bits (black)

    (SETSCREEN) // Set D to equal the new status before jumping here
    @ARG
    M=D         // Save the new status argument 
    @status     // FFFF=black, 0=white - status of entire screen
    D=D-M       // D=newstatus-status
    @LOOP
    D;JEQ       // Do nothing if the new status equals the old status

    @ARG
    D=M
    @status
    M=D         // status = ARG

    @SCREEN
    D=A         // D=Screen address
    @8192
    D=D+A       // D=Byte past the last screen address
    @i
    M=D         // i=SCREEN address

    (SETLOOP)
    @i
    D=M-1
    M=D        // i=i-1 
    @LOOP
    D;JLT      // if i<0 goto LOOP

    @status
    D=M        // D=status
    @i
    A=M        // Indirect 
    M=D        // Set the current screen address to the new status
    @SETLOOP
    0;JMP