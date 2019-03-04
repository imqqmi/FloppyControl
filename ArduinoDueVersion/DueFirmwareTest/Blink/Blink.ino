/*
  Blink

  Turns an LED on for one second, then off for one second, repeatedly.

  Most Arduinos have an on-board LED you can control. On the UNO, MEGA and ZERO
  it is attached to digital pin 13, on MKR1000 on pin 6. LED_BUILTIN is set to
  the correct LED pin independent of which board is used.
  If you want to know what pin the on-board LED is connected to on your Arduino
  model, check the Technical Specs of your board at:
  https://www.arduino.cc/en/Main/Products

  modified 8 May 2014
  by Scott Fitzgerald
  modified 2 Sep 2016
  by Arturo Guadalupi
  modified 8 Sep 2016
  by Colby Newman

  This example code is in the public domain.

  http://www.arduino.cc/en/Tutorial/Blink
*/



unsigned char buf[16768];
byte rxbuf[64];
String str;
int capture = 0;
int rxLength = 0;

void setup() {
  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, LOW);
  SerialUSB.begin(0);
  while (!SerialUSB);
  SerialUSB.setTimeout(100);
  for( long i = 0; i < 16768; i++)
    buf[i] = (unsigned char)(i & 0xff);
}

void loop() {
  if( capture == 1)
  {  
    long len = SerialUSB.availableForWrite();
    //delay(1);
    if( len == 511  );
      SerialUSB.write(buf, 8192);
  }
  
  rxLength = SerialUSB.available();
  if( rxLength > 0 )
    SerialUSB.readBytes(rxbuf, rxLength);
  
  if( rxbuf[0] == ',' )
  {  
    capture = 1;
    digitalWrite(LED_BUILTIN, HIGH);
  }
  
  if( rxbuf[0] == '.' )
  {
    capture = 0;
    digitalWrite(LED_BUILTIN, LOW);
  }

  
  //if (SerialUSB.available()) 
  //serialUSBEvent();

}

void serialUSBEvent() {
    
    SerialUSB.write(str.c_str());
}
