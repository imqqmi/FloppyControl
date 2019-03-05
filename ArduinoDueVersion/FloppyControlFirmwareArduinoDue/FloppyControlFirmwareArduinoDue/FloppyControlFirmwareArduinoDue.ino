/*
    FloppyControlFirmware for Arduino Due

    Written by Josha Beukema 2019

    Last change: 2019-03-02

    This firmware aims to control a floppy disk drive and capture the period times of the
    Read signal. This is passed through the native usb virtual serial port for fast
    transfer to the Windows PC FloppyControl application written in C#.

    The firmware is controlled by one letter commands:
    a/q select/deselect drive
    s/w enable/disable motor
    d/e forward/backward direction of the drive head
    f/r active low/high step signal
    t/g next track (single STEP=0/STEP=1 combination)/previous track
    j/h side=0/side=1
    ./, stop capture/start capture
    [/] increment/decrement microstepping for step stick
    y   next track based on microstepping
    0   Seek TRK00
    i   Display info, to check in a terminal client to see if it works

    Serial connection is at 5000000 baud (5mbit).
    
*/
#define TX_BUF_SIZE   8192 // double buffer, so 2x this value is used in ram

#include "tc_lib.h"

using namespace arduino_due;

#define CAPTURE_TIME_WINDOW 10 // usecs
#define ANALOG_PIN 7
#define ANALOG_VALUE 127 // values in the interval [0,255] 

// Pin definitions
// Outputs                floppy connector
#define MOTEA           3 // p10
#define DRVSB           4 // p12
#define MOTEB           5 // p16

#define DIR             6 // p18
#define STEP            7 // p20
#define WDATE           9 // p22
#define WGATE           13// p24
#define SIDE            8 // p32

//Inputs, IOC
#define READY           59 // p2
#define INDEX           10 // p8  
#define TRK00           11 // p26
#define RDATA           2  // p30


#define _STEPSTICKEN    54 //A0
#define MS1             55 //A1
#define MS2             56 //A2
#define MS3             57 //A3
#define RESET           58 //A4


#define RDATAIE         INTE

#define TRACKDELAY      3

// capture_tc0 declaration
// IMPORTANT: Take into account that for TC0 (TC0 and channel 0) the TIOA0 is
// PB25, which is pin 2 for Arduino DUE, so  the capture pin in  this example
// is pin 2. For the correspondence between all TIOA inputs for the different 
// TC modules, you should consult uC Atmel ATSAM3X8E datasheet in section "36. 
// Timer Counter (TC)"), and the Arduino pin mapping for the DUE.
capture_tc0_declaration();
auto& capture_pin2=capture_tc0;

byte rxbuf[64];
String str;
int capture = 0;
int rxLength = 0;
char recvchar = 0;
char recvbuf;
char curtrk = 0;
char microstep = 1;

volatile byte* buf;
unsigned long bufindex;
volatile bool buf1ready;
volatile bool buf2ready;
volatile char stop = 1;

void setup() {
  bufindex = 0;
  buf1ready = false;
  buf1ready = false;
  pinMode( LED_BUILTIN, OUTPUT);
  // Outputs
  pinMode( MOTEA, OUTPUT);
  pinMode( DRVSB, OUTPUT);
  pinMode( MOTEB, OUTPUT);
  pinMode( DIR, OUTPUT);
  pinMode( STEP, OUTPUT);
  pinMode( SIDE, OUTPUT);
  
  //Inputs, IOC
  pinMode( READY , INPUT);
  pinMode( INDEX , INPUT); // interrupt on change pin
  pinMode( TRK00, INPUT);
  pinMode( RDATA, INPUT); // Capture period times
  //Stepstick outputs
  pinMode( _STEPSTICKEN, OUTPUT);
  pinMode( MS1, OUTPUT);
  pinMode( MS2, OUTPUT);
  pinMode( MS3, OUTPUT);
  pinMode( RESET, OUTPUT);

  digitalWrite(LED_BUILTIN, LOW);

  capture_pin2.config(CAPTURE_TIME_WINDOW);

  analogWrite(ANALOG_PIN,ANALOG_VALUE);
  buf = capture_pin2.get_buf();
  setMS();

  //Turn off disk drive
  digitalWrite( DRVSB, 1); // DRVSB = 1;
  digitalWrite( RESET, 0); //RESET = 0;
  digitalWrite( MOTEA, 1); //MOTEB = 1;
  digitalWrite( MOTEB, 1); //MOTEB = 1;
  
  SerialUSB.begin(0);
  while (!SerialUSB);
  SerialUSB.setTimeout(100);
}

void loop() {
  
  if( stop == 0)
  {  
    capture_pin2.get_bufready(buf1ready, buf2ready);
    if( buf1ready )
    {
      long len = SerialUSB.availableForWrite();
      if( len == 511  ) SerialUSB.write((byte *)buf, TX_BUF_SIZE);
      capture_pin2.reset_buf1ready();
    }
    if( buf2ready )
    {
      long len = SerialUSB.availableForWrite();
      if( len == 511  ) SerialUSB.write((byte *)buf+TX_BUF_SIZE, TX_BUF_SIZE);
      capture_pin2.reset_buf2ready();
    }
  }

  // Read from serial port
  rxLength = SerialUSB.available();
  if( rxLength > 0 )
  {
    SerialUSB.readBytes(&recvbuf, 1);
    menu();
  }
}

void menu(void)
{
    char step;
    
    //if( recvchar )
    {
        recvchar = 0;
        switch(recvbuf)
        {

            case 'q': // Deselect drive
                //txstring("x12 DRVSB enabled\r\n\0");
                digitalWrite( DRVSB, 1); // DRVSB = 1;
                //digitalWrite( MOTEA, 1); //SELECT0 = 1;
                digitalWrite( RESET, 0); //RESET = 0;
            break;
            case 'w': // Disable  motor
                //txstring("16 MOTEB enabled\r\n");
                digitalWrite( MOTEA, 1); //MOTEB = 1;
                digitalWrite( MOTEB, 1); //MOTEB = 1;
                digitalWrite( _STEPSTICKEN, 1); //_STEPSTICKEN = 1;
            break; 
            case 'e': // DIR = 1
                //txstring("DIR enabled\r\n");
                digitalWrite( DIR, 1); // DIR = 1;
            break;
            case 'r':
                //txstring("STEP enabled\r\n");
                digitalWrite( STEP, 1); //STEP = 1;
            break;                
//**************************************************************
            case 'a': // Select drive
                //txstring("12 DRVSB disabled\r\n");
                digitalWrite( DRVSB, 0); //DRVSB = 0;
                //digitalWrite( MOTEA, 1); //SELECT0 = 1;
                digitalWrite( _STEPSTICKEN, 1); //RESET = 1;
            break;
            case 's': // Enable motor and stepstick
                //txstring("16 MOTEB disabled\r\n");
                digitalWrite( MOTEB, 0); //MOTEB = 0;
                digitalWrite( MOTEA, 0); //MOTEA = 0;
                digitalWrite( _STEPSTICKEN, 0); //_STEPSTICKEN = 0;
            break;
            case 'd': // DIR = 0
                //txstring("DIR disabled\r\n");
                digitalWrite( DIR, 0); //DIR = 0;
            break;
            case 'f':
                //txstring("STEP disabled\r\n");
                digitalWrite( STEP, 0); //STEP = 0;
            break;
//***************************************************************            
            case '0': // Seek track 00
                //txstring("Seeking track 00..\r\n");
                curtrk = 0;
                seektrk00();
            break;
            case '8': // Goto track 80
                //txstring("Goto track 80..\r\n");
                gototrack(80);
            break;
            case 'v': // dump data to console
            break;
            case 't': // next track
                curtrk++;
                inctrack();
            break;
             case 'y': // next track
                curtrk+=microstep;
                for( step = 0; step < microstep; step++)
                    inctrack();
            break;           
            case 'g': // previous track
                curtrk--;
                digitalWrite( DIR, 1); //DIR = 1; // decrement
    
                digitalWrite( STEP, 0); //STEP = 0; // pulse a step, 2us pulses
                delayMicroseconds(6);
                digitalWrite( STEP, 1); //STEP = 1;
                delay(TRACKDELAY); // wait 20ms between pulses
                //CLRWDT();
            break;
            case '.': // 
                // stop received
                //txstring("Stop=1\r\n");
                digitalWrite(LED_BUILTIN, LOW);
                stop = 1;

            break;
            case ',': // Continue capture
                // stop received
                //txstring("Stop=0\r\n\0");
                digitalWrite(LED_BUILTIN, HIGH);
                stop = 0;

            break;
            case 'h': // HEAD 0
                
                digitalWrite( SIDE, 1); //SIDE = 1;

            break;
            case 'j': // HEAD 1
                
                digitalWrite( SIDE, 0); //SIDE = 0;

            break;
            case '[':
                microstep++;
                setMS();
                break;
            case ']':
                microstep = 1;
                setMS();
                break;
            
            case 'i':
                SerialUSB.print("Arduino Due working!!!");
                break;
            default:
                break;
        }
    }
}

// Set microstepping depending on the user chonsen microstep
void setMS()
{
    unsigned char ms;
    switch(microstep)
    {
        case 1:
            ms = 0b000;
            break;
        case 2:
            ms = 0b011;
            break;
        case 4:
            ms = 0b010;
            break;
        case 8:
            ms = 0b110;
            break;
        case 16:
            ms = 0b111;
            break;

        default: 
            ms = 0b000; // full step
            break;
    }
    digitalWrite( MS1, (ms & 0b100) >> 2); // MS1 = (ms & 0b100) >> 2;
    digitalWrite( MS2, (ms & 0b010) >> 1); // MS2 = (ms & 0b010) >> 1;
    digitalWrite( MS3, (ms & 0b001));      // MS3 = (ms & 0b001);
}

// Seek TRK00
void seektrk00(void) 
{
    int i,j;
    
    digitalWrite( DIR, 1); //DIR = 1; // decrement until trk00 is found

    // Set the microstepping to full step
    digitalWrite( MS1, 0); // MS1 = 0;
    digitalWrite( MS2, 0); // MS2 = 0;
    digitalWrite( MS3, 0); // MS3 = 0;
    
    
    
    // Quick seek for speed
    for( i=0; i<180; i++)
    {
        if( !digitalRead(TRK00) ) //If the interrupt triggered, exit the loop
            break;
        
        digitalWrite( STEP, 0); //STEP = 0; // pulse a step, 10 us pulses
        delayMicroseconds(20);
        digitalWrite( STEP, 1); //STEP = 1;
        delay(3); // wait 20ms between pulses
    }
    
    // move head forward again
    digitalWrite( DIR, 0); //DIR = 0;
    for( i=0; i<10; i++)
    {
        digitalWrite( STEP, 0); //STEP = 0; // pulse a step, 2us pulses
        delayMicroseconds(20); 
        digitalWrite( STEP, 1); //STEP = 1;
        delay(3); 
    }

    // Seek slowly for accuracy
    digitalWrite( DIR, 1); //DIR = 1;
    for( i=0; i<250; i++)
    {
        if( !digitalRead(TRK00) ) //If the interrupt triggered, exit the loop
            break;
        
        digitalWrite( STEP, 0); //STEP = 0; // pulse a step, 10 us pulses
        delayMicroseconds(100);
        digitalWrite( STEP, 1); //STEP = 1;
        delay(15); // wait 20ms between pulses
    }

    // return the microstepping to the user set value
    setMS();
}

// Go to track t, seeking first track 00
void gototrack(char t)
{
    char i,j;
    
    unsigned int number_of_steps;
    unsigned int cnt;

    digitalWrite( DIR, 1); //DIR = 1; // decrement until trk00 is found
    
    // Home head to track 00
    seektrk00();
    // Set the microstepping to full step
    digitalWrite( MS1, 0); // MS1 = 0;
    digitalWrite( MS2, 0); // MS2 = 0;
    digitalWrite( MS3, 0); // MS3 = 0;
    
    digitalWrite( DIR, 0); //DIR = 0;
    // Go to requisted track
    
    number_of_steps = t * 2;
    for( cnt = 0; cnt <number_of_steps; cnt++)
    {
        
        digitalWrite( STEP, 0); //STEP = 0; // pulse a step, 2us pulses
        delay(1); 
        digitalWrite( STEP, 1); //STEP = 1;
        delay(2); 
    }
    
    setMS();
}

// Increment 1 track, assuming the motor is spinning
void inctrack(void)
{
    digitalWrite( DIR, 0); //DIR = 0;
    // Go to requested track
        
    digitalWrite( STEP, 0); //STEP = 0; // pulse a step, 2us pulses
    delayMicroseconds(6);
    digitalWrite( STEP, 1); //STEP = 1;
    delay(TRACKDELAY); // wait 20ms between pulses
}
