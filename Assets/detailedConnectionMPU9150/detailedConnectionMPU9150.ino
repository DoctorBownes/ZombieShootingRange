
#include "I2Cdev.h"

#include "MPU6050_6Axis_MotionApps20.h"
#if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
#include "Wire.h"
#endif
MPU6050 mpu;
#define OUTPUT_READABLE_QUATERNION
#define LED_PIN 13 // (Arduino is 13, Teensy2.0 is 11, Teensy2.0++ is 6)
bool blinkState = false;
#define INTERRUPT_PIN  2
#define ISSUE_COMMAND  'b'
#define SET_LED_ON     'x'
#define SET_LED_OFF    'y'

// MPU control/status vars
bool dmpReady = false;  // set true if DMP init was successful
uint8_t mpuIntStatus;   // holds actual interrupt status byte from MPU
uint8_t devStatus;      // return status after each device operation (0 = success, !0 = error)
uint16_t packetSize;    // expected DMP packet size (default is 42 bytes)
uint16_t fifoCount;     // count of all bytes currently in FIFO
uint8_t fifoBuffer[64]; // FIFO storage buffer

// orientation/motion vars
Quaternion q;           // [w, x, y, z]         quaternion container
float euler[3];         // [psi, theta, phi]    Euler angle container
int button1 = 12;
int buttonState1 = 0;
float xValue1 = 0;
float yValue1 = 0;
float xValue2 = 0;
float yValue2 = 0;
char btnpressed = 0;
bool btnrelease = true;
unsigned long previousMillis = 0;        // will store last time LED was updated
const long DZmillis = 1000;

// packet structure for InvenSense teapot demo
uint8_t teapotPacket[14] = { '$', 0x02, 0, 0, 0, 0, 0, 0, 0, 0, 0x00, 0x00, '\r', '\n' };

// ================================================================
// ===               INTERRUPT DETECTION ROUTINE                ===
// ================================================================

volatile bool mpuInterrupt = false;     // indicates whether MPU interrupt pin has gone high
void dmpDataReady() {
  mpuInterrupt = true;
}

// ================================================================
// ===                      INITIAL SETUP                       ===
// ================================================================
void setup() {
  btnpressed = 0;
  pinMode(INTERRUPT_PIN, INPUT); // Gidi: sets the digital pin as input,  necessary for the interrupt

  // join I2C bus (I2Cdev library doesn't do this automatically)
#if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
  Wire.begin();
  TWBR = 24; // 400kHz I2C clock (200kHz if CPU is 8MHz)
#elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
  Fastwire::setup(400, true);
#endif

  Serial.begin(115200);
  // Gidi: commented out the next while loop because it gave errors and I am not using Arduino Leonardo
  // while (!Serial);

  // initialize device
  Serial.println(F("Initializing I2C devices..."));
  mpu.initialize();

  // verify connection
  Serial.println(F("Testing device connections..."));
  Serial.println(mpu.testConnection() ? F("MPU6050 connection successful") : F("MPU6050 connection failed"));

  // wait for ready
  delay(5000);
  Serial.println(F("Initializing DMP..."));
  devStatus = mpu.dmpInitialize();
  if (devStatus == 0) {
    // turn on the DMP, now that it's ready
    Serial.println(F("Enabling DMP..."));
    mpu.setDMPEnabled(true);

    attachInterrupt(INTERRUPT_PIN, dmpDataReady, RISING);    // (Gidi) if you use the interrupts, you can set this to interrupt 2 for the Teensy2.0
    mpuIntStatus = mpu.getIntStatus();

    dmpReady = true;

    // get expected DMP packet size for later comparison
    packetSize = mpu.dmpGetFIFOPacketSize();
    //Wait for accelerometer ready
    delay(15000);
  } else {
    Serial.print(F("DMP Initialization failed (code "));
    Serial.print(devStatus);
    Serial.println(F(")"));
  }
  // configure LED for output
  pinMode(LED_PIN, OUTPUT);
  pinMode(button1, INPUT_PULLUP);
}

// ================================================================
// ===                    MAIN PROGRAM LOOP                     ===
// ================================================================

void loop() {
  if (blinkState == false)
  {
    digitalWrite(LED_PIN, HIGH);
  }
  else
  {
    digitalWrite(LED_PIN, LOW);
  }
  // if programming failed, don't try to do anything
  if (!dmpReady) return;

  // wait for MPU interrupt or extra packet(s) available
  while (!mpuInterrupt && fifoCount < packetSize) {
    if (Serial.available()) {
      char inByte = Serial.read();
      if (inByte == ISSUE_COMMAND) {
        // parse commands from computer
        parseCommands();
      }  // else, ignore this byte
    }


  }


  // reset interrupt flag and get INT_STATUS byte
  mpuInterrupt = false;
  mpuIntStatus = mpu.getIntStatus();

  // get current FIFO count
  fifoCount = mpu.getFIFOCount();

  // check for overflow (this should never happen unless our code is too inefficient)
  if ((mpuIntStatus & 0x10) || fifoCount == 1024) {
    // reset so we can continue cleanly
    mpu.resetFIFO();
    //Serial.println(F("FIFO overflow!"));

    // otherwise, check for DMP data ready interrupt (this should happen frequently)
  } else if (mpuIntStatus & 0x02) {
    // wait for correct available data length, should be a VERY short wait
    while (fifoCount < packetSize) fifoCount = mpu.getFIFOCount();

    // read a packet from FIFO
    mpu.getFIFOBytes(fifoBuffer, packetSize);

    // track FIFO count here in case there is > 1 packet available
    // (this lets us immediately read more without waiting for an interrupt)
    fifoCount -= packetSize;

#ifdef OUTPUT_READABLE_QUATERNION
    // display quaternion values in easy matrix form: w x y z
    buttonState1 = digitalRead(button1);
    mpu.dmpGetQuaternion(&q, fifoBuffer); // Gidi: we just read the quaternion q, and process later
    euler[0] = atan2(2 * (q.w * q.z + q.x * q.y), 1 - 2 * (q.y * q.y + q.z * q.z)) * -1;
    euler[1] = atan2(2 * (q.w * q.x + q.y * q.z), 1 - 2 * (q.x * q.x + q.y * q.y)) * -1;
    euler[2] = 2 * (q.w * q.y - q.z * q.x);
    Serial.print("Euler X :");
    Serial.print(euler[0]);

    Serial.print("\t");
    Serial.print(euler[1]);
    Serial.print("\t");
    Serial.println(euler[2]);
    unsigned long currentMillis = millis();

    if (buttonState1 == LOW && btnrelease == true) {
      if (currentMillis - previousMillis >= 200) {
        // save the last time you blinked the LED
        if (btnpressed < 2)
        {
          if (btnpressed == 0)
          {
            xValue1 = euler[0];
            yValue1 = euler[1];
          }
          if (btnpressed == 1)
          {
            xValue2 = euler[0];
            yValue2 = euler[1];
            blinkState = true;
          }
          btnpressed++;
        }
        else
        {
              Mouse.press(MOUSE_LEFT);
              Mouse.release(MOUSE_LEFT);
        }
        previousMillis = currentMillis;
        btnrelease = false;
      }
    }
    else if (buttonState1 == HIGH)
    {
        btnrelease = true;
    }
        if (btnpressed >= 2)
        {

          if ((euler[0] > xValue1 && euler[0] <= xValue2) && (euler[1] <= yValue1 && euler[1] > yValue2))
          {
            euler[0] = map(euler[0], xValue1, xValue2, 0, 1920);
            euler[1] = map(euler[1], yValue1, yValue2, 0, 1080);
            Serial.println("Within proximity!");
            //Mouse.moveTo(euler[0] * (1920 / ((xValue2 - xValue1))), euler[1] * (1080 / (yValue2 - yValue1)));
            Mouse.moveTo(euler[0], euler[1]);
          }
          else
          {
            Serial.println("Outside proximity!");
          }
        }

#endif
      }

    }

    // parse commands received from the computer
    void parseCommands() {
      // read next byte as command
      delay(1);  // a small delay seems necessary to get Serial.available working again?
      if (Serial.available()) {
        char inByte = Serial.read();
        if (inByte == SET_LED_ON) {
          digitalWrite(LED_PIN, HIGH);   // set the LED on
        }
        else if (inByte == SET_LED_OFF) {
          digitalWrite(LED_PIN, LOW);   // set the LED off
        }  // else, ignore this byte
      }
    }
