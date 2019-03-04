## tc_lib v1.1

This is tc_lib library for the Arduino DUE electronic prototyping platform. 

Copyright (C) 2015,2016 Antonio C. Domínguez Brito (<adominguez@iusiani.ulpgc.es>). División de Robótica y Oceanografía Computacional (<http://www.roc.siani.es>) and Departamento de Informática y Sistemas (<http://www.dis.ulpgc.es>). Universidad de Las Palmas de Gran  Canaria (ULPGC) (<http://www.ulpgc.es>).
  
### 0. License 

The tc_lib library is an open source project which is openly available under the GNU General Public License (GPL) license.

### 1. Introduction

This is a C++ library to take advantage of some of the features available on TC (Timer Counter) modules in DUE's Atmel ATSAM3X8E micro-controller.

In tc_lib current version you can use two kind of objects, capture and action objects. Each object is associated to one of the three channels available at each of the three TC available in the micro-controller. In total nine timer/counter channels denoted by TC0, TC1, TC2, TC3, TC4, TC5, TC6, TC7 and TC8, respectively. Take into account that on each TC module and channel you can only use one tc_lib object for correct operation, whether a capture or an action object.

### 2. Capture objects

Capture objects allow measuring period and pulses (the duty) of digital signals, like PWM signals. This is carried out in parallel using the resources on TC's channels.

Concretely, it is possible to use up to nine capture objects each one corresponding to the nine channels available, namely, TC0, TC1, TC2 (on TC module 0), TC3, TC4, TC5 (on TC module 1), and TC6, TC7 and TC8 (on TC module 2). For using any of them you have to previously declare them in advance using a specific declaration syntax provided by the library through macros *capture_tc0_declaration*, *capture_tc1_declaration*, etc. A snippet of code extracted from the example *capture_test.ino* declaring a capture object corresponding to TC0 is shown next:

```
// capture_tc0 declaration
// IMPORTANT: Take into account that for TC0 (TC0 and channel 0) the TIOA0 is
// PB25, which is pin 2 for Arduino DUE, so  the capture pin in  this example
// is pin 2. For the correspondence between all TIOA inputs for the different 
// TC modules, you should consult uC Atmel ATSAM3X8E datasheet in section "36. 
// Timer Counter (TC)", and the Arduino pin mapping for the DUE.

capture_tc0_declaration();
```

Example *capture_test.ino* illustrates how to use a capture object. Once declared you have to config the capture object specifying its capture window in microseconds. The capture window specifies how long will be the capture object waiting for the completion of a pulse. For example, if you are trying to capture a PWM signal with a specific period, the capture window should be established to be the double of the expected period for correct behavior. Here a fragment of code extracted from the same example:

```
// capture_tc0 initialization
capture_tc0.config(CAPTURE_TIME_WINDOW);
```

As soon as the object is configured the corresponding TC channel starts capturing the digital signal present on the capture input pins associated with that specific channel. Those pins are hardwired to the TC channels, to know which ones are corresponding to each TC channel you have to consult the *Timer Counter (TC)* section of Atmel ATSAM3X8E datasheet (<http://www.atmel.com/devices/SAM3X8E.aspx>), and the mapping of micro-controller pins on the DUE (<https://www.arduino.cc/en/Hacking/PinMappingSAM3X>). For example *capture_test.ino*, the pin associated with TC0 channel is TIOA0 which is PB25 micro-controller pin, that in turn is pin 2 on the DUE platform.

Capture objects take advantage of TC module channels in *capture* mode to measure digital pulses. Concretely, using tc_lib capture objects we can obtain the last pulse duration (duty) and the last period of the measured signal. Example *capture_test.ino* use a PWM signal generated on analog pin 7 as the signal to measure. When nothing is measured the duty and the period measured by the capture object are zero. Take into account that the measured duty and period get stored inside the capture objects at the interrupt handlers associated with the TC channels involved.

In addition, capture objects can tell you how many pulses they have captured so far using member function *get_duty_period_and_pulses()*.

### 2.1. Fast signals and capture objects

When capturing signals capture objects do intensive use of interrupts associated to the TC channel associated with the specific object. If the signal to capture is very fast (pulse duration around 1 microsecond or less), some interrupts will be missed to track the signal. Internally the TC channel in capture mode registers those situations with the signaling of a "load overrun" of one of the capture registers RA or RB (more details in ATSAM3X8E data sheet). Evidently, this may also happen in an application where the use of interrupts is very high, even if the signal to capture is not so fast. In any case, specially with fast signals (frequencies of around 1 Mhz) this massive use of interrupts could provoke the freezing of the CPU, since all CPU time was invested on interrupts. To avoid that situation, the capture object stops capturing when it detects too much overrun events, keeping internally the duty and period of the last pulse captured. Function *get_duty_and_period()* returns a status value where we can check if the capture objects was overrun and/or stopped. Here a snippet of code from example *pwm_capture_test.ino* illustrating its use:

```
status=capture_pin2.get_duty_and_period(duty,period); <====
Serial.print("duty: "); 
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
Serial.print(" usecs. ");
if(capture_pin2.is_overrun(status)) Serial.print("[overrun]"); <====
if(capture_pin2.is_stopped(status)) Serial.print("[stopped]"); <==== 
Serial.println();   
```
Once a capture object is stopped due to the occurrence of too many load overrun situations, the object is restarted when calling member function *get_duty_and_period()* or member function *restart()*.

Note that many load overruns implies that the capture object is losing pulses, accordingly member function *get_duty_period_and_pulses()* will provide only the number of pulses really captured.

## 3. Action objects

The action objects available in tc_lib allow us to have a callback called periodically in our program.

Action objects are associated with a specific TC channel, whether TC0, TC1, TC2, TC3, TC4, TC5, TC6, TC7 and TC8, and with each channel it is only possible to use only one action object (or only one capture object). Equally to capture objects, before its use you must declare it using a specific macro, namely, *action_tc0_declaration*, *action_tc1_declaration*, ect. Here we include a snippet from example *action_test.ino* declaring *action_tc0* object:

```
// action_tc0 declaration
action_tc0_declaration();
```

To start using an action object we have to call action object's *start()* member function to establish the callback and the period (in hundredths of microseconds) to call the callback. Here a fragment of code illustrating its use in example *action_test.ino*:

```
action_tc0.start(CALLBACK_PERIOD,set_led_action,&action_ctx);
```

We can stop the action object calling the callback at any moment calling member function stop:

```
action_tc0.stop(); // stopping
```

### 4. Download & installation

The  library  is  available  through  an  open	git  repository  available   at:

* <https://github.com/antodom/tc_lib>

and also at our own mirror at:

* <http://bizet.dis.ulpgc.es/antodom/tc_lib>

For using it you just have to copy the library on the libraries folder used by your Arduino IDE, the folder should be named "tc_lib".

In addition you must add the flag -std=gnu++11 for compiling. For doing that add -std=gnu++11 to the platform.txt file, concretely to compiler.cpp.flags. In Arduino IDE 1.6.6 and greater versions this flag is already set.

### 5. Examples

On the examples directory you have available a basic example for using a capture object, *capture_test.ino*, and baseci example for using action objects, *action_test.ino*. Example *wave_example.ino* shows how to use action objects to send a byte through a pin using a digital wave. And finally a fourth example, *pwm_capture_test.ino* which is specifically designed to check the capture objects with fast signals. For this example it is necessary the use of library pwm_lib, available at [https://github.com/antodom/pwm_lib](https://github.com/antodom/pwm_lib).

I hope all four examples are self-explaining.

### 6. Incompatibilities

Any library using the interrupts associated with any of the timer/counter module channels TC0, TC1, TC2, TC3, TC4, TC5, TC6, TC7 and TC8, has a potential compatibility problem with tc_lib, if it happens to use the same TC module and the same channel. An example of this potential incompatibility is library Servo which uses by default TC0, TC2, TC3, TC4 and TC5 interrupt handlers. In this case, you will be limited to use the capture and/or action objects associated with TC channels TC1, TC6, TC7 and TC8, to preserve the compatibility.

### 7. Compiling with CMake

For compiling on command line using CMake, just proceed in the following manner:

1. Set the following environment variables (a good place to put them is on .bashrc):
  * Set `ARDUINO_DUE_ROOT_PATH` to `~/.arduino15/packages/arduino/`.
  * Set `ARDUINO_DUE_VERSION` to `1.6.17`.
  * Set `ARDUINO_UNO_ROOT_PATH` to the path where you have installed the Arduino IDE, for example, `~/installations/arduino-1.6.5`.
  * Set `ARDUINO_IDE_LIBRARY_PATH` to the path for the Arduino IDE projects you have in preferences. For example, `~/arduino_projects`.

2. Go to `tc_lib` directory (the one you have cloned with the project).
3. Create directory `build` (if not already created).
4. Execute `cmake ..`.
5. Set the following flags and variables (if not done before), you should execute `ccmake ..`:
  * Set `PORT` to the serial port you will use for uploading.
  * Set `IS_NATIVE_PORT` to `true` (if using DUE's native port) or `false` (if using DUE's programming port).
6. Be sure the changes were applied, usually running `cmake ..`.
7. Compile executing `make`.
8. The previous step has generated the examples available with the library. You can upload the code executing:
  * `make upload_capture_test`, 
  * `make upload_action_test`, 
  * `make upload_pwm_capture_test`, 
  * `make upload_wave_example`, 

### 8. Library users

In this section we would like to enumerate users using the library in their own projects and developments. So please, if you are using the library, drop us an email indicating in what project or development you are using it.

The list of users/projects goes now:

1. **Project:** Autonomous sailboat A-Tirma (<http://velerorobot.blogspot.com.es>). **User**: División de Robótica y Oceanografía Computacional (<http://www.roc.siani.es>). **Description**: The library was a specific development for this project. The sailboat onboard system is based on an Arduino DUE. 

### 9. Feedback & Suggestions

Please be free to send me any comment, doubt of use, or suggestion in relation to tc_lib.
