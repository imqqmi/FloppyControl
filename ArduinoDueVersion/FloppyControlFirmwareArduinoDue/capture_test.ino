/**
 ** tc_lib library
 ** Copyright (C) 2016
 **
 **   Antonio C. Domínguez Brito <adominguez@iusiani.ulpgc.es>
 **     División de Robótica y Oceanografía Computacional <www.roc.siani.es>
 **     and Departamento de Informática y Sistemas <www.dis.ulpgc.es>
 **     Universidad de Las Palmas de Gran  Canaria (ULPGC) <www.ulpgc.es>
 **  
 ** This file is part of the tc_lib library.
 ** The tc_lib library is free software: you can redistribute it and/or modify
 ** it under  the  terms of  the GNU  General  Public  License  as  published  by
 ** the  Free Software Foundation, either  version  3  of  the  License,  or  any
 ** later version.
 ** 
 ** The  tc_lib library is distributed in the hope that  it  will  be  useful,
 ** but   WITHOUT   ANY WARRANTY;   without   even   the  implied   warranty   of
 ** MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE.  See  the  GNU  General
 ** Public License for more details.
 ** 
 ** You should have received a copy  (COPYING file) of  the  GNU  General  Public
 ** License along with the tc_lib library.
 ** If not, see: <http://www.gnu.org/licenses/>.
 **/
/*
 * File: capture_test.ino 
 * Description: This is an example illustrating the use of the tc_lib library's
 * capture objects. Concretely captures the PWM output from an analog pin esta-
 * blished with an analog write.
 * Date: November 25th, 2015
 * Author: Antonio C. Dominguez-Brito <adominguez@iusiani.ulpgc.es>
 * ROC-SIANI - Universidad de Las Palmas de Gran Canaria - Spain
 */

#include "tc_lib.h"

using namespace arduino_due;

#define CAPTURE_TIME_WINDOW 2500000 // usecs
#define ANALOG_PIN 7
#define ANALOG_VALUE 127 // values in the interval [0,255] 

// capture_tc0 declaration
// IMPORTANT: Take into account that for TC0 (TC0 and channel 0) the TIOA0 is
// PB25, which is pin 2 for Arduino DUE, so  the capture pin in  this example
// is pin 2. For the correspondence between all TIOA inputs for the different 
// TC modules, you should consult uC Atmel ATSAM3X8E datasheet in section "36. 
// Timer Counter (TC)"), and the Arduino pin mapping for the DUE.
capture_tc0_declaration();
auto& capture_pin2=capture_tc0;

volatile byte* buf;
unsigned long bufindex;

void setup() {
  // put your setup code here, to run once:
  bufindex = 0;
  Serial.begin(9600);

  // capture_pin2 initialization
  capture_pin2.config(CAPTURE_TIME_WINDOW);

  analogWrite(ANALOG_PIN,ANALOG_VALUE);
  buf = capture_pin2.get_buf();
}

void loop() {

  uint32_t status,duty,period;
  
  status=capture_pin2.get_duty_and_period(duty,period);
  
  Serial.println("********************************************************");  
  Serial.print("--> [PIN "); Serial.print(ANALOG_PIN);
  Serial.print(" -> PIN 2] duty: "); 
  Serial.print(
    static_cast<double>(duty)/
    static_cast<double>(capture_pin2.ticks_per_usec()),
    3
  );
  Serial.print(" usecs. period: ");
  Serial.print( static_cast<double>(period)/ static_cast<double>(capture_pin2.ticks_per_usec()), 3);
  Serial.print(" \r\nperiod ticks: ");
  Serial.print( buf[bufindex++] );
  if( bufindex == 16768) bufindex = 0;
  
}
