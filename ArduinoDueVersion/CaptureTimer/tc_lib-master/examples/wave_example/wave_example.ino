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
 * File: wave_test.ino 
 * Description: This is an example illustrating the use of the tc_lib library's
 * action objects for generating a digital signal.  
 * Date: February 21st, 2017
 * Author: Antonio C. Dominguez-Brito <adominguez@iusiani.ulpgc.es>
 * ROC-SIANI - Universidad de Las Palmas de Gran Canaria - Spain
 */

#include "tc_lib.h"

using namespace arduino_due;

#define WAVE_PERIOD 120 //460 // 1200 //50000 // hundredths of usecs. (1e-8 msecs.)
#define WAVE_PIN 10 

#define CAPTURE_TIME_WINDOW 2500000 // usecs

#define DELAY_TIME 2000 // msecs.

// action_tc1 declaration, we use TC1 channel
action_tc1_declaration();
auto& action_obj=action_tc1;

// capture_tc0 declaration
// NOTE: we will use capture_tc0 to verify the wave signal we will generate,
// using this example, in order to verify that the measured timing are right
// IMPORTANT: Take into account that for TC0 (TC0 and channel 0) the TIOA0 is
// PB25, which is pin 2 for Arduino DUE, so  the capture pin in  this example
// is pin 2. For the correspondence between all TIOA inputs for the different 
// TC modules, you should consult uC Atmel ATSAM3X8E datasheet in section "36. 
// Timer Counter (TC)"), and the Arduino pin mapping for the DUE.
capture_tc0_declaration();
auto& capture_pin2=capture_tc0;

template<typename Action_Object>
class wave
{
  public:

    wave(Action_Object& action_obj,uint32_t pin,uint32_t period)
    : _bit_(8), 
      _action_obj_(action_obj), 
      _period_(period),
      _pin_(pin)
    {
      _pio_p_=g_APinDescription[_pin_].pPort;
      _mask_=g_APinDescription[_pin_].ulPin;

      // setting pin as output
      pinMode(_pin_,HIGH);
    }

    bool is_sending() { return (_bit_<8); }
    
    bool send(uint8_t data)
    {
      // only when the last data has been sent 
      // we accept another byte 
      if(is_sending()) return false;

      _data_=data; _bit_=0;
     
      return action_obj.start(
        _period_,
        _send_bit_callback_,
        reinterpret_cast<void*>(this)
      );
    }

  private:
    
    volatile uint8_t _data_;
    volatile uint32_t _bit_;
    volatile uint32_t _period_;

	  uint32_t _pin_;
	  Pio* _pio_p_;
	  uint32_t _mask_;

    Action_Object& _action_obj_;
  
    static void _send_bit_callback_(void* ptr)
    {
      wave* this_ptr=reinterpret_cast<wave*>(ptr);
      
      this_ptr->_send_bit_();
    }
    
    void _send_bit_()
    {
      if(_data_ & (1<<_bit_))
	      PIO_Set(_pio_p_,_mask_);
	    else PIO_Clear(_pio_p_,_mask_);

      _bit_++;
      //_bit_&=0x3;
      if(_bit_>=8) action_obj.stop(); 
    }
};

wave<action_tc1_t> wave_obj(action_tc1,WAVE_PIN,WAVE_PERIOD);

void setup() {
  // put your setup code here, to run once:

  Serial.begin(9600);

  // capture_pin2 initialization
  capture_pin2.config(CAPTURE_TIME_WINDOW);

  while(!wave_obj.send(0b01010101)) {}

  Serial.println("========================================================");

  Serial.print("max period: "); 
  Serial.print(action_tc1.max_period());
  Serial.println(" hundreths of usecs.");
  Serial.print("max period ticks: "); 
  Serial.println(action_tc1.ticks(action_tc1.max_period()));
  Serial.print("period: "); 
  Serial.print(action_tc1.get_period());
  Serial.println(" hundreths of usecs. (1e-8 secs.)");
  Serial.print("ticks: "); 
  Serial.println(action_tc1.ticks(WAVE_PERIOD));

  Serial.println("========================================================");

}

void loop() {
  // put your main code here,to run repeatedly:

  uint32_t status,duty,period;
  status=capture_pin2.get_duty_and_period(duty,period);

  while(!wave_obj.send(0b01010101)) {}

  Serial.println("********************************************************");  
  Serial.print("--> [PIN "); Serial.print(WAVE_PIN);
  Serial.print(" -> PIN 2] duty: "); 
  Serial.print(
    static_cast<double>(duty)/
    static_cast<double>(capture_pin2.ticks_per_usec()),
    3
  );
  Serial.print(" usecs. period: ");
  Serial.print(
    static_cast<double>(period)/
    static_cast<double>(capture_pin2.ticks_per_usec()),
    3
  );
  Serial.println(" usecs.");
}


