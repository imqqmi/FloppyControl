/*!
@file DPLL.cs
<summary>DPLL Class: implements a DPLL similar to the one used in the WD1772 FDC</summary>

 * This Digital Phase Lock Loop uses the algorithm described in US patent 4,780,844\n
 * http://info-coach.fr/atari/documents/general/patents/US4780844.pdf
	
<div class="jlg">Copyright (C) 2013-2015 Jean Louis-Guerin\n\n
This file is part of the Atari Universal FD Image Tool project.\n\n
The Atari Universal FD Image Tool project may be used and distributed without restriction provided
that this copyright statement is not removed from the file and that any
derivative work contains the original copyright notice and the associated
disclaimer.\n\n
The Atari Universal FD Image Tool project is free software; you can redistribute it
and/or modify  it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 3
of the License, or (at your option) any later version.\n\n
The Atari Universal FD Image Tool project is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.\n
See the GNU General Public License for more details.\n\n
You should have received a copy of the GNU General Public License
along with the Atari Universal FD Image Tool project; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA\n</div>
	
@author Jean Louis-Guerin
	
*/

namespace FDCPackage {

/// <summary>The PLL Class implements a FD controller DPLL</summary>
public class DPLL {
	/// <summary> 8 MHz clock = 125 ns, I use 50ns periods - Josha Beukema</summary>
	const int PLL_CLK = 80;

	/// <summary>phase compensation</summary>
	const int PHASE_CORRECTION = 90;

    /// <summary>low_value for phase compensation</summary>
    const int LOW_CORRECTION = 128 - PHASE_CORRECTION;

    /// <summary>high value for phase compensation</summary>
    const int HIGH_CORRECTION = 128 + PHASE_CORRECTION;

    /// <summary>bottom value for counter (128 - 10%) = 115</summary>
    const int LOW_STOP = 115;
	
	/// <summary>top value for counter (128 + 10%) = 141</summary>
	const int HIGH_STOP = 141;	

	/// <summary> running time</summary>
	private int _currentTime;

	/// <summary>increment counter</summary>
	private bool _up;

	/// <summary>decrement counter</summary>
	private bool _down;

	/// <summary>select LOW_FIX value</summary>
	private bool _low;

	/// <summary>select HIGH_FIX value</summary>
	private bool _high;	

	/// <summary>period_count initialized to 128 (128*16=2048=2^11)</summary>
	private int _count;

	/// <summary>11 bits adder</summary>
	private int _adder;

	/// <summary>lead/lag (msb) history (previous 2 msb)</summary>
	private int _history;

	/// <summary> freq correction amount</summary>
	private int _freqAmount;	

	/// <summary>phase amount correction</summary>
	private int _phaseAmount;

	/// <summary>Floppy density True=HD False=DD</summary>
	private bool _density;

	/// <summary>get current PLL clock period in nanoseconds </summary>
	/// <returns>The actual period of the adjusted clock in nanoseconds</returns>
	public int BitWidth {
		get {
			// clock = (2048 * 125ns) / period_count
			int clock = 256000 / _count;
			if (_density)
				return clock / 2;
			return clock;
		}
	}


 	/// <summary>The DPLL class Constructor. We reset all internals values</summary>
	/// <param name="den">Set to true for HD MFM FD</param>
	public DPLL(bool den = false) {
		_currentTime = 0;			// reset current time
		_density = den;				// set density;
		reset();
	}

	private void reset() {
		_up = false;					// increment counter false
		_down = false;				// decrement counter false
		_count = 128;				// period count initialized to 128 (128*16=2048=2^11)
		_adder = 0;					// reset the 11 bits adder
		_low = false;				// select LOW_FIX false
		_high = false;				// select HIGH_FIX false
		_history = 0;				// reset history
		_freqAmount = 0;				// reset freq correction amount
		_phaseAmount = 0;			// reset phase amount correction
	}


	/// <summary>Find in which inspection window the data pulse is located</summary>
	/// <param name="dataTime">absolute time in nanoseconds of the 
	/// data pulse since the start of the track</param>
	/// <returns>The number of 2µs inspection windows to sample to the next 
	/// data pulse. Normal values are 2,3,4 (corresponding to 4, 6, 8µs 
	/// but any value can be returned.</returns>
	/// <remarks>The DPLL adjust the frequency (duration) and the phase 
	/// (start / stop time) of the inspection window based on the history 
	/// of previously received pulses to correct frequency and/or phase 
	/// variation.
	/// The window size (fixed by periodCount) changed based on MSB error history.
	///	The data position (phase) in window changed based on adder MSB to
	///	try to move data pulse in middle of inspection window
	public int bitSpacing(int dataTime) {
		bool dataNotFound = true;	// set to false if data found in window
		bool readPulse = false;		// set to true for one loop
		int error = 0;				// MSB + history (3 bits)
		int numWindows = 0;			// number of inspection windows to find data pulse

		// trick => if in High Density we multiply the value by 2 ==> clock / 2
		if (_density) dataTime *= 2;

		// here we check for Looong flux like in NFA
		if ((dataTime - _currentTime) > 256 * PLL_CLK) {	// 32 µS
			int nw = (dataTime - _currentTime) / (16 * PLL_CLK);
			_currentTime = dataTime;
			reset();
			return nw;
		}

		// do while the data pulse is not found
		do {
			numWindows++;
			// do while no roll over in the 11 bits adder
			do {
				_currentTime += PLL_CLK;
				// up for one loop only
				if (readPulse)
					readPulse = false;

				// check data in inspection window
				if (_currentTime >= dataTime) {
					if (dataNotFound)			// one time
						readPulse = true;
					dataNotFound = false;		// data found
				}

				// adjust period_count based on frequency correction
				if (_up && (_count < HIGH_STOP))		_count++;
				if (_down && (_count > LOW_STOP))	_count--;

                // update adder based on phase correction
                if (_low)       _adder += LOW_CORRECTION;
                else if (_high) _adder += HIGH_CORRECTION;
                else            _adder += _count;
					
				if (readPulse) {
					// -------------------- //
					// frequency correction //
					// -------------------- //

					if ((_adder & 0x400) == 0) switch(_history) {		// adder msb == 0
							case 0: error = 2; break;	// 000 very fast
							case 1: error = 1; break;	// 001 fast
							case 2: error = 0; break;	// 010 ok
							case 3: error = 0; break;	// 011 now adjusted
						}	// adder msb == 0
					else switch (_history) {							// adder msb == 1
							case 0: error = 0; break;	// 100 now adjusted
							case 1: error = 0; break;	// 101 ok
							case 2: error = 1; break;	// 110 slow
							case 3: error = 2; break;	// 111 very slow
						}	// adder msb == 1


					// compute and save new history based on adder msb and previous history
					_history = ((_adder & 0x400) >> 9) | (_history >> 1) & 0x7F;

					// set freq_amount based on error history and msb
					// The frequency decoder controls the period of the data inspection window respective 
					// to the error for the 11 bit adder is as follows:
					// error = 0: 
					//				-> no correction necessary <-
					// error = 1:
					//				MSBs input:			 7	 6	 5	 4	 3	 2	 1	 0
					//				Correction output:  -3  -2  -1   0   0  +1  +2  +3
					// error = 2:
					//				MSBs input:			 7	 6	 5	 4	 3	 2	 1	 0
					//				Correction output:  -4  -3  -2  -1  +1  +2  +3  +4
					// The most significant bit of the freq_amount controls increment (value 0)
					// or decrement (value 1) of the adder
					switch (error) {
						case 2:
							switch ((_adder >> 8) & 0x7) {
								case 0: _freqAmount = 0x4; break;	// 0100
								case 1: _freqAmount = 0x3; break;	// 0011
								case 2: _freqAmount = 0x2; break;	// 0010
								case 3: _freqAmount = 0x1; break;	// 0001
								case 4: _freqAmount = 0x9; break;	// 1001
								case 5: _freqAmount = 0xA; break;	// 1010
								case 6: _freqAmount = 0xB; break;	// 1011
								case 7: _freqAmount = 0xC; break;	// 1100
							}
							break;

						case 1:
							switch ((_adder >> 8) & 0x7) {
								case 0: _freqAmount = 0x3; break;	// 0011
								case 1: _freqAmount = 0x2; break;	// 0010
								case 2: _freqAmount = 0x1; break;	// 0001
								case 3: _freqAmount = 0x0; break;	// 0000
								case 4: _freqAmount = 0x8; break;	// 1000
								case 5: _freqAmount = 0x9; break;	// 1001
								case 6: _freqAmount = 0xA; break;	// 1010
								case 7: _freqAmount = 0xB; break;	// 1011
							}
							break;

						case 0: 
							_freqAmount = 0;
							break;
					} // set freq amount based on history

					// ---------------- //
					// Phase correction //
					// ---------------- //
					// set phase amount based on adder msbs
					// The phase decoder depends on the value of ADDER_MSBs. If the phase leads, 
					// the most significant bit of phase_amount indicates with a '0', that the next 
					// rollover should appear earlier. In case of a phase lag, the next rollover 
					// should come later (indicated by a '1' of the most significant bit of phase_amount
					// bit 3 used to control high(0) or low(1) select
					switch ((_adder >> 8) & 0x7) {	// adder 3 msb
						case 0: _phaseAmount = 0x4; break;		// 000 huge lead
						case 1: _phaseAmount = 0x3; break;		// 001 large lead
						case 2: _phaseAmount = 0x2; break;		// 010 medium lead
						case 3: _phaseAmount = 0x1; break;		// 011 small lead
						case 4: _phaseAmount = 0x9; break;		// 100 small lag
						case 5: _phaseAmount = 0xA; break;		// 101 medium lag
						case 6: _phaseAmount = 0xB; break;		// 110 large lag
						case 7: _phaseAmount = 0xC; break;		// 111 huge lag
					}
				} // readPulse == true

				else {
					if ((_freqAmount & 0x7) != 0)	_freqAmount--;
					if ((_phaseAmount & 0x7) != 0)	_phaseAmount--;
				} // readPulse == false

				// set freq correction flags
				if ((_freqAmount & 0x07) != 0) {
					// freq correction needed
					if ((_freqAmount & 0x8) != 0) {
						_up = false;
						_down = true;
					}
					else {
						_up = true;
						_down = false;
					}
				} // freq amount != 0
				else {
					// no freq correction needed
					_up = false;
					_down = false;
				} // freq amount == 0

				// set phase correction flags
				if ((_phaseAmount & 0x7) != 0) {
					// phase correction needed
					if ((_phaseAmount & 0x8) != 0) {
						_low = true;
						_high = false;
					}
					else {
						_high = true;
						_low = false;
					}
				} // phase correction needed
				else {
					_high = false;
					_low = false;
				}

			} while (_adder < 2048); // do until adder roll over
			_adder -= 2048;

		} while (dataNotFound); // do until we found data
		return numWindows;
	} // end of dataInWindow

}	// end of class DPLL

}	// name space

