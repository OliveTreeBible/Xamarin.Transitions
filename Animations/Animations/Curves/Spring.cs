/* Port of the algorithm behind http://facebook.github.io/rebound/. 
    While the same calculations are in place, it has been rewritten to leverage yield return and IEnumerable
    to drive the resolution of the Spring. License for Rebound:

BSD License

For Rebound software

Copyright (c) 2013, Facebook, Inc.
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

- Redistributions of source code must retain the above copyright notice, this list of conditions and the following
  disclaimer.
  
- Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided with the distribution.
  
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/


using System;
using System.Collections.Generic;

namespace OliveTree.Animations.Curves
{
    public class Spring : AnimationCurve
    {
        public int Tension { get; set; } = 40;
        public int Friction { get; set; } = 7;

        public IEnumerable<double> Steps(int stepsPerSecond)
        {
            //Port of http://facebook.github.io/rebound/
            const double restVelocityThreshold = .001,
                distanceFromRestThreshold = .001,
                startValue = 0,    //we're building up an easing curve from 0-1ish, not end values
                endValue = 1.0;

            double stepTime = 1.0 / stepsPerSecond,
                halfStep = stepTime / 2.0;

            //Maps Origami parameters to this algorithm
            double tension = (Tension - 30.0) * 3.62 + 194.0,
                friction = (Friction - 8.0) * 3.0 + 25.0;
            double position = startValue,
                velocity = 0,
                seedPosition = 0;

            yield return startValue;
            do
            {
                var aVel = velocity;
                var aAcc = tension * (endValue - seedPosition) - friction * velocity;

                double bVel = velocity + aAcc * halfStep,
                    bAcc = tension * (endValue - (position + aVel * halfStep)) - friction * bVel;

                double cVel = velocity + bAcc * halfStep,
                    cAcc = tension * (endValue - (position + bVel * halfStep)) - friction * cVel;

                seedPosition = position + cVel * stepTime;
                double dVel = velocity + cAcc * stepTime,
                    dAcc = tension * (endValue - seedPosition) - friction * dVel;

                double dxdt = 1.0 / 6.0 * (aVel + 2.0 * (bVel + cVel) + dVel),
                    dvdt = 1.0 / 6.0 * (aAcc + 2.0 * (bAcc + cAcc) + dAcc);

                position += dxdt * stepTime;
                velocity += dvdt * stepTime;

                yield return position;
            } while (!(Math.Abs(velocity) < restVelocityThreshold) ||
                    (!(Math.Abs(endValue - position) <= distanceFromRestThreshold) && Math.Abs(tension) > .00001));
            yield return endValue;
        }
    }
}