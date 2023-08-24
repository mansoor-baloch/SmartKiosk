using ITLlib;
using SmartKioskApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SmartKioskApp
{
    public class CNV11
    {
        static int
            GivingChangeCounter,
            DispensingCounter = 0;


        // ssp library variables
        SSPComms m_eSSP;
        SSP_COMMAND m_cmd;
        SSP_KEYS keys;
        SSP_FULL_KEY sspKey;
        SSP_COMMAND_INFO info;

        // variable declarations
        //CCommsWindow m_Comms;

        // The protocol version this validator is using, set in setup request
        int m_ProtocolVersion;

        // Variables to hold the number of notes accepted and dispensed
        int m_TotalNotesAccepted, m_TotalNotesDispensed;

        // The number of channels used in this validator
        int m_NumberOfChannels;

        // The multiplier by which the channel values are multiplied to get their
        // true penny value.
        int m_ValueMultiplier;

        // The type of unit this class represents, set in the setup request
        char m_UnitType;

        // Integer array to hold the value of each note stored in the payout
        int[] m_NotePositionValues;

        //String to hold the currency stored in the note float
        string m_StoredCurrency;

        //Integer to hold total number of Hold messages to be issued before releasing note from escrow
        int m_HoldNumber;

        //Integer to hold number of hold messages still to be issued
        int m_HoldCount;

        //Bool to hold flag set to true if a note is being held in escrow
        bool m_NoteHeld;

        // Current poll response and length
        byte[] m_CurrentPollResponse;
        byte m_CurrentPollResponseLength;

        // A list of dataset data, sorted by value. Holds the info on channel number, value, currency,
        // level and whether it is being recycled.
        List<ChannelData> m_UnitDataList;
        //MotorController MC;
        // constructor
        public CNV11()
        {
            m_eSSP = new SSPComms();
            m_cmd = new SSP_COMMAND();
            keys = new SSP_KEYS();
            sspKey = new SSP_FULL_KEY();
            info = new SSP_COMMAND_INFO();

            //m_Comms = new CCommsWindow();
            m_TotalNotesAccepted = 0;
            m_TotalNotesDispensed = 0;
            m_NumberOfChannels = 0;
            m_ValueMultiplier = 1;
            m_CurrentPollResponse = new byte[256];
            m_CurrentPollResponseLength = 0;
            m_UnitDataList = new List<ChannelData>();
            m_NotePositionValues = new int[30];
            m_StoredCurrency = "";
            m_HoldCount = 0;
            m_HoldNumber = 0;
            //MC = new MotorController();
        }

        /* Variable Access */

        // access to ssp vars
        public SSPComms SSPComms
        {
            get { return m_eSSP; }
            set { m_eSSP = value; }
        }

        public SSP_COMMAND CommandStructure
        {
            get { return m_cmd; }
            set { m_cmd = value; }
        }

        public SSP_COMMAND_INFO InfoStructure
        {
            get { return info; }
            set { info = value; }
        }

        // access to comms
        //public CCommsWindow CommsLog
        //{
        //    get { return m_Comms; }
        //    set { m_Comms = value; }
        //}
        // access to notes accepted
        public int NotesAccepted
        {
            get { return m_TotalNotesAccepted; }
            set { m_TotalNotesAccepted = value; }
        }

        // access to notes dispensed
        public int NotesDispensed
        {
            get { return m_TotalNotesDispensed; }
            set { m_TotalNotesDispensed = value; }
        }

        // access to number of channels
        public int NumberOfChannels
        {
            get { return m_NumberOfChannels; }
            set { m_NumberOfChannels = value; }
        }

        // access to value multiplier
        public int Multiplier
        {
            get { return m_ValueMultiplier; }
            set { m_ValueMultiplier = value; }
        }

        // access to the list of data items
        public List<ChannelData> UnitDataList
        {
            get { return m_UnitDataList; }
        }

        // access to currency stored in note float
        public string StoredCurrency
        {
            get { return m_StoredCurrency; }
            set { m_StoredCurrency = value; }
        }

        // acccess to hold count
        public int HoldNumber
        {
            get { return m_HoldNumber; }
            set { m_HoldNumber = value; }

        }

        public bool NoteHeld
        {
            get { return m_NoteHeld; }
            //set { m_HoldNumber = value; }

        }


        // get a channel value
        public int GetChannelValue(int channelNum)
        {
            if (channelNum >= 1 && channelNum <= m_NumberOfChannels)
            {
                foreach (ChannelData d in m_UnitDataList)
                {
                    if (d.Channel == channelNum)
                        return d.Value;
                }
            }
            return -1;
        }
        // get a channel currency
        public string GetChannelCurrency(int channelNum)
        {
            if (channelNum >= 1 && channelNum <= m_NumberOfChannels)
            {
                foreach (ChannelData d in m_UnitDataList)
                {
                    if (d.Channel == channelNum)
                        return new string(d.Currency);
                }
            }
            return "";
        }
        /* Command functions */

        // The enable command allows the validator to receive and act on commands sent to it.
        public void EnableValidator(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_ENABLE;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return;
            // check response
            if (CheckGenericResponses(log) && log != null) { }
            //log.AppendText("Validator enabled\r\n");
        }

        // Disable command stops the validator from acting on commands.
        public void DisableValidator(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_DISABLE;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return;
            // check response
            if (CheckGenericResponses(log) && log != null) { }
            //log.AppendText("Validator disabled\r\n");
        }

        // Enable payout allows the validator to payout and store notes.
        public void EnablePayout(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_ENABLE_PAYOUT_DEVICE;
            m_cmd.CommandData[1] = 0x01; // second byte to enable note value to be sent with stored event
            m_cmd.CommandDataLength = 2;

            if (!SendCommand(log)) return;
            if (CheckGenericResponses(log) && log != null) { }
            //log.AppendText("Payout enabled\r\n");
        }

        // Disable payout stops the validator being able to store/payout notes.
        public void DisablePayout(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_DISABLE_PAYOUT_DEVICE;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return;
            if (CheckGenericResponses(log) && log != null) { }
            //log.AppendText("Payout disabled\r\n");
        }

        // Empty payout device takes all the notes stored and moves them to the cashbox.
        public void EmptyPayoutDevice(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_EMPTY_ALL;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return;
            if (CheckGenericResponses(log))
            {
                if (log != null) { } //log.AppendText("Emptying payout device\r\n");
            }
        }

        // Payout last note command takes the last note paid in and dispenses it first (LIFO system). 
        public void PayoutNextNote(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_PAYOUT_NOTE;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand(log)) return;

            if (CheckGenericResponses(log))
            {
                if (log != null) { } //log.AppendText("Paying out next note\r\n");
            }
        }

        // Return Note command returns note held in escrow to bezel. 
        public void ReturnNote(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_REJECT_BANKNOTE;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand(log)) return;

            if (CheckGenericResponses(log))
            {
                if (log != null)
                {
                    //log.AppendText("Returning note\r\n");
                }
                m_HoldCount = 0;
            }
        }

        // Smart Empty command places all stored notes into cashbox and returns interim and total values
        // of notes transferred.
        public void SmartEmpty(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SMART_EMPTY;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand(log)) return;

            if (CheckGenericResponses(log))
            {
                if (log != null)
                {
                    //log.AppendText("Smart Empty In Progress\r\n");
                }
            }
        }

        // Set value reporting type changes the validator to return either the value of the note, or the channel it is stored on
        // depending on what byte is sent after the command (0x01 is channel, 0x00 is 4 bit value).
        public void SetValueReportingType(bool byChannel, TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_VALUE_REPORTING_TYPE;
            if (byChannel)
                m_cmd.CommandData[1] = 0x01; // report by channel number
            else
                m_cmd.CommandData[1] = 0x00; // report by 4 bit value
            m_cmd.CommandDataLength = 2;

            if (!SendCommand(log)) return;
            if (CheckGenericResponses(log) && log != null) { }
            //log.AppendText("Value reporting type changed\r\n");
        }

        // The set routing command changes the way the validator deals with a note, either it can send the note straight to the cashbox
        // or it can store the note for payout. This is specified in the second byte (0x00 to store for payout, 0x01 for cashbox). The 
        // bytes after this represent the 4 bit value of the note, or the channel (see SetValueReportingType()).
        // This function allows the note to be specified as an int in the param note, the stack bool is true for cashbox, false for storage.
        public void ChangeNoteRoute(int note, char[] currency, bool stack, TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_DENOMINATION_ROUTE;

            // if this note is being changed to stack (cashbox)
            if (stack)
                m_cmd.CommandData[1] = 0x01;
            // note being stored (payout)
            else
                m_cmd.CommandData[1] = 0x00;

            // get the note as a byte array
            byte[] b = BitConverter.GetBytes(note);
            m_cmd.CommandData[2] = b[0];
            m_cmd.CommandData[3] = b[1];
            m_cmd.CommandData[4] = b[2];
            m_cmd.CommandData[5] = b[3];

            // send country code
            m_cmd.CommandData[6] = (byte)currency[0];
            m_cmd.CommandData[7] = (byte)currency[1];
            m_cmd.CommandData[8] = (byte)currency[2];

            m_cmd.CommandDataLength = 9;

            if (!SendCommand(log)) return;
            if (CheckGenericResponses(log) && log != null)
            {
                string s;
                string cs = new string(currency);
                if (stack) s = " to cashbox)\r\n";
                else s = " to storage)\r\n";
                //log.AppendText("Note routing successful (" + CHelpers.FormatToCurrency(note) + cs + s);
            }
        }

        //// This function sends the command LAST REJECT CODE which gives info about why a note has been rejected. It then
        //// outputs the info to a passed across textbox.
        //public void QueryRejection(TextBox log)
        //{
        //    m_cmd.CommandData[0] = CCommands.SSP_CMD_LAST_REJECT_CODE;
        //    m_cmd.CommandDataLength = 1;
        //    if (!SendCommand(log)) return;

        //    if (CheckGenericResponses(log))
        //    {
        //        if (log == null) return;
        //        switch (m_cmd.ResponseData[1])
        //        {
        //            case 0x00: //log.AppendText("Note accepted\r\n"); break;
        //            case 0x01: //log.AppendText("Note length incorrect\r\n"); break;
        //            case 0x02: //log.AppendText("Invalid note 0x2\r\n"); break;
        //            case 0x03: //log.AppendText("Invalid note 0x03\r\n"); break;
        //            case 0x04: //log.AppendText("Invalid note 0x04\r\n"); break;
        //            case 0x05: //log.AppendText("Invalid note 0x05\r\n"); break;
        //            case 0x06: //log.AppendText("Channel inhibited\r\n"); break;
        //            case 0x07: //log.AppendText("Second note inserted during read\r\n"); break;
        //            case 0x08: //log.AppendText("Host rejected note\r\n"); break;
        //            case 0x09: //log.AppendText("Invalid note 0x09\r\n"); break;
        //            case 0x0A: //log.AppendText("Invalid note read 0x0A\r\n"); break;
        //            case 0x0B: //log.AppendText("Note too long\r\n"); break;
        //            case 0x0C: //log.AppendText("Validator disabled\r\n"); break;
        //            case 0x0D: //log.AppendText("Mechanism slow/stalled\r\n"); break;
        //            case 0x0E: //log.AppendText("Strim attempt\r\n"); break;
        //            case 0x0F: //log.AppendText("Fraud channel reject\r\n"); break;
        //            case 0x10: //log.AppendText("No notes inserted\r\n"); break;
        //            case 0x11: //log.AppendText("Peak detect fail\r\n"); break;
        //            case 0x12: //log.AppendText("Twisted note detected\r\n"); break;
        //            case 0x13: //log.AppendText("Escrow time-out\r\n"); break;
        //            case 0x14: //log.AppendText("Bar code scan fail\r\n"); break;
        //            case 0x15: //log.AppendText("Rear sensor fail 2\r\n"); break;
        //            case 0x16: //log.AppendText("Slot fail 1\r\n"); break;
        //            case 0x17: //log.AppendText("Slot fail 2\r\n"); break;
        //            case 0x18: //log.AppendText("Lens over-sample\r\n"); break;
        //            case 0x19: //log.AppendText("Incorrect note width\r\n"); break;
        //            case 0x1A: //log.AppendText("Note too short\r\n"); break;
        //        }
        //    }
        //}
        public void QueryRejection(TextBox log)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_LAST_REJECT_CODE;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand(log)) return;

            if (CheckGenericResponses(log))
            {
                switch (m_cmd.ResponseData[1])
                {
                    case 0x00: Console.WriteLine("Note accepted\r\n"); break;
                    case 0x01: Console.WriteLine("Note length incorrect\r\n"); break;
                    case 0x02:
                        Console.WriteLine("Invalid note 0x2\r\n");
                        Global.NoteValidator.SSP_QUERY_REJECTION_INVALID_NOTE = true;
                        break;
                    case 0x03:
                        Console.WriteLine("Invalid note 0x03\r\n");
                        Global.NoteValidator.SSP_QUERY_REJECTION_INVALID_NOTE = true;
                        break;

                    case 0x04:
                        Console.WriteLine("Invalid note 0x04\r\n");
                        Global.NoteValidator.SSP_QUERY_REJECTION_INVALID_NOTE = true;
                        break;
                    case 0x05:
                        Console.WriteLine("Invalid note 0x05\r\n");
                        Global.NoteValidator.SSP_QUERY_REJECTION_INVALID_NOTE = true;
                        break;
                    case 0x06: Console.WriteLine("Channel inhibited\r\n"); break;
                    case 0x07: Console.WriteLine("Second note inserted during read\r\n"); break;
                    case 0x08: Console.WriteLine("Host rejected note\r\n"); break;
                    case 0x09:
                        Console.WriteLine("Invalid note 0x09\r\n");
                        Global.NoteValidator.SSP_QUERY_REJECTION_INVALID_NOTE = true;
                        break;
                    case 0x0A:
                        Console.WriteLine("Invalid note read 0x0A\r\n");
                        Global.NoteValidator.SSP_QUERY_REJECTION_INVALID_NOTE = true;
                        break;
                    case 0x0B: Console.WriteLine("Note too long\r\n"); break;
                    case 0x0C: Console.WriteLine("Validator disabled\r\n"); break;
                    case 0x0D:
                        Console.WriteLine("Mechanism slow/stalled\r\n");
                        Global.NoteValidator.SSP_QUERY_REJECTION_MECHNISM_SLOW_STALLED = true;
                        break;
                    case 0x0E: Console.WriteLine("Strim attempt\r\n"); break;
                    case 0x0F: Console.WriteLine("Fraud channel reject\r\n"); break;
                    case 0x10: Console.WriteLine("No notes inserted\r\n"); break;
                    case 0x11: Console.WriteLine("Peak detect fail\r\n"); break;
                    case 0x12:
                        Console.WriteLine("Twisted note detected\r\n");
                        Global.NoteValidator.SSP_QUERY_REJECTION_TWISTED_NOTE_DETECTED = true;
                        break;
                    case 0x13: Console.WriteLine("Escrow time-out\r\n"); break;
                    case 0x14: Console.WriteLine("Bar code scan fail\r\n"); break;
                    case 0x15: Console.WriteLine("Rear sensor fail 2\r\n"); break;
                    case 0x16: Console.WriteLine("Slot fail 1\r\n"); break;
                    case 0x17: Console.WriteLine("Slot fail 2\r\n"); break;
                    case 0x18: Console.WriteLine("Lens over-sample\r\n"); break;
                    case 0x19: Console.WriteLine("Incorrect note width\r\n"); break;
                    case 0x1A: Console.WriteLine("Note too short\r\n"); break;
                }
            }
        }
        // The get note positions command instructs the validator to return in the second byte the number of
        // notes stored and then in the following bytes, the values/channel (see SetValueReportingType()) of the stored
        // notes. The length of the response will vary based on the number of stored notes.
        public void CheckForStoredNotes(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_GET_NOTE_POSITIONS;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand(log)) return;
            if (CheckGenericResponses(log))
            {
                int counter = 0;
                Array.Clear(m_NotePositionValues, 0, m_NotePositionValues.Length);
                // Work backwards for a more accurate display (LIFO)
                for (int i = (m_cmd.ResponseData[1] * 4) + 1; i >= 2; i -= 4, counter++)
                    m_NotePositionValues[counter] = CHelpers.ConvertBytesToInt32(m_cmd.ResponseData, i - 3);
            }
        }

        // The stack last note command is similar to the payout last note command (PayoutNextNote()) except it
        // moves the stored notes to the cashbox instead of dispensing.
        public void StackNextNote(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_STACK_NOTE;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand(log)) return;

            if (CheckGenericResponses(log) && log != null)
            {
                //log.AppendText("Moved note from storage to cashbox\r\n");
            }
        }

        // The reset command instructs the validator to restart (same effect as switching on and off)
        public void Reset(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_RESET;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return;
            CheckGenericResponses(log);
        }

        // This function sets the protocol version in the validator to the version passed across. Whoever calls
        // this needs to check the response to make sure the version is supported.
        public void SetProtocolVersion(byte pVersion, TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_HOST_PROTOCOL_VERSION;
            m_cmd.CommandData[1] = pVersion;
            m_cmd.CommandDataLength = 2;
            if (!SendCommand(log)) return;
        }

        // This function performs a number of commands in order to setup the encryption between the host and the validator.
        public bool NegotiateKeys(TextBox log = null)
        {
            // make sure encryption is off
            m_cmd.EncryptionStatus = false;

            // send sync
            if (log != null) //log.AppendText("Syncing... ");
                m_cmd.CommandData[0] = CCommands.SSP_CMD_SYNC;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return false;
            if (log != null) //log.AppendText("Success\r\n");

                m_eSSP.InitiateSSPHostKeys(keys, m_cmd);

            // send generator
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_GENERATOR;
            m_cmd.CommandDataLength = 9;
            if (log != null) //log.AppendText("Setting generator... ");

                // Convert generator to bytes and add to command data.
                BitConverter.GetBytes(keys.Generator).CopyTo(m_cmd.CommandData, 1);

            if (!SendCommand(log)) return false;
            if (log != null) //log.AppendText("Success\r\n");

                // send modulus
                m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_MODULUS;
            m_cmd.CommandDataLength = 9;
            if (log != null) //log.AppendText("Sending modulus... ");

                // Convert modulus to bytes and add to command data.
                BitConverter.GetBytes(keys.Modulus).CopyTo(m_cmd.CommandData, 1);

            if (!SendCommand(log)) return false;
            if (log != null) //log.AppendText("Success\r\n");

                // send key exchange
                m_cmd.CommandData[0] = CCommands.SSP_CMD_REQUEST_KEY_EXCHANGE;
            m_cmd.CommandDataLength = 9;
            if (log != null) //log.AppendText("Exchanging keys... ");

                // Convert host intermediate key to bytes and add to command data.
                BitConverter.GetBytes(keys.HostInter).CopyTo(m_cmd.CommandData, 1);


            if (!SendCommand(log)) return false;
            if (log != null) //log.AppendText("Success\r\n");

                // Read slave intermediate key.
                keys.SlaveInterKey = BitConverter.ToUInt64(m_cmd.ResponseData, 1);

            m_eSSP.CreateSSPHostEncryptionKey(keys);

            // get full encryption key
            m_cmd.Key.FixedKey = 0x0123456701234567;
            m_cmd.Key.VariableKey = keys.KeyHost;

            if (log != null) { } //log.AppendText("Keys successfully negotiated\r\n");

            return true;
        }

        // This function uses the setup request command to get all the information about the validator.
        public void ValidatorSetupRequest(TextBox log = null)
        {
            StringBuilder sbDisplay = new StringBuilder(1000);

            // send setup request
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SETUP_REQUEST;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return;

            // display setup request


            // unit type
            int index = 1;
            sbDisplay.Append("Unit Type: ");
            m_UnitType = (char)m_cmd.ResponseData[index++];
            switch (m_UnitType)
            {
                case (char)0x00: sbDisplay.Append("Validator"); break;
                case (char)0x03: sbDisplay.Append("SMART Hopper"); break;
                case (char)0x06: sbDisplay.Append("SMART Payout"); break;
                case (char)0x07: sbDisplay.Append("NV11"); break;
                case (char)0x0D: sbDisplay.Append("TEBS"); break;
                default: sbDisplay.Append("Unknown Type"); break;
            }

            // firmware
            sbDisplay.AppendLine();
            sbDisplay.Append("Firmware: ");
            while (index <= 5)
            {
                sbDisplay.Append((char)m_cmd.ResponseData[index++]);
                if (index == 4)
                    sbDisplay.Append(".");
            }
            sbDisplay.AppendLine();
            // country code.
            // legacy code so skip it.
            index += 3;

            // value multiplier.
            // legacy code so skip it.
            index += 3;

            // Number of channels
            sbDisplay.AppendLine();
            sbDisplay.Append("Number of Channels: ");
            m_NumberOfChannels = m_cmd.ResponseData[index++];
            sbDisplay.Append(m_NumberOfChannels);
            sbDisplay.AppendLine();

            // channel values.
            // legacy code so skip it.
            index += m_NumberOfChannels; // Skip channel values

            // channel security
            // legacy code so skip it.
            index += m_NumberOfChannels;

            // real value multiplier
            // (big endian)
            sbDisplay.Append("Real Value Multiplier: ");
            m_ValueMultiplier = m_cmd.ResponseData[index + 2];
            m_ValueMultiplier += m_cmd.ResponseData[index + 1] << 8;
            m_ValueMultiplier += m_cmd.ResponseData[index] << 16;
            sbDisplay.Append(m_ValueMultiplier);
            sbDisplay.AppendLine();
            index += 3;


            // protocol version
            sbDisplay.Append("Protocol Version: ");
            m_ProtocolVersion = m_cmd.ResponseData[index++];
            sbDisplay.Append(m_ProtocolVersion);
            sbDisplay.AppendLine();

            // Add channel data to list then display.
            // Clear list.
            m_UnitDataList.Clear();
            // Loop through all channels.

            for (byte i = 0; i < m_NumberOfChannels; i++)
            {
                ChannelData loopChannelData = new ChannelData();
                // Channel number.
                loopChannelData.Channel = (byte)(i + 1);

                // Channel value.
                loopChannelData.Value = BitConverter.ToInt32(m_cmd.ResponseData, index + (m_NumberOfChannels * 3) + (i * 4)) * m_ValueMultiplier;

                // Channel Currency
                loopChannelData.Currency[0] = (char)m_cmd.ResponseData[index + (i * 3)];
                loopChannelData.Currency[1] = (char)m_cmd.ResponseData[(index + 1) + (i * 3)];
                loopChannelData.Currency[2] = (char)m_cmd.ResponseData[(index + 2) + (i * 3)];

                // Channel level.
                loopChannelData.Level = 0;

                // Channel recycling
                IsNoteRecycling(loopChannelData.Value, loopChannelData.Currency, ref loopChannelData.Recycling);

                // Add data to list.
                m_UnitDataList.Add(loopChannelData);

                //Display data
                sbDisplay.Append("Channel ");
                sbDisplay.Append(loopChannelData.Channel);
                sbDisplay.Append(": ");
                sbDisplay.Append(loopChannelData.Value / m_ValueMultiplier);
                sbDisplay.Append(" ");
                sbDisplay.Append(loopChannelData.Currency);
                sbDisplay.AppendLine();
            }

            // Sort the list by .Value.
            m_UnitDataList.Sort((d1, d2) => d1.Value.CompareTo(d2.Value));

            if (log != null) { }
            //log.AppendText(sbDisplay.ToString());
        }

        // This function sends the set inhibits command to set the inhibits on the validator. An additional two
        // bytes are sent along with the command byte to indicate the status of the inhibits on the channels.
        // For example 0xFF and 0xFF in binary is 11111111 11111111. This indicates all 16 channels supported by
        // the validator are uninhibited. If a user wants to inhibit channels 8-16, they would send 0x00 and 0xFF.
        public void SetInhibits(TextBox log = null)
        {
            // set inhibits
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_CHANNEL_INHIBITS;
            m_cmd.CommandData[1] = 0xFF;
            m_cmd.CommandData[2] = 0xFF;
            m_cmd.CommandDataLength = 3;

            if (!SendCommand(log)) return;
            if (CheckGenericResponses(log) && log != null)
            {
                //log.AppendText("Inhibits set\r\n");
            }
        }

        // This function uses the GET ROUTING command to determine whether a particular note
        // is recycling.
        void IsNoteRecycling(int note, char[] currency, ref bool b, TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_GET_DENOMINATION_ROUTE;
            byte[] byteArr = CHelpers.ConvertIntToBytes(note);
            m_cmd.CommandData[1] = byteArr[0];
            m_cmd.CommandData[2] = byteArr[1];
            m_cmd.CommandData[3] = byteArr[2];
            m_cmd.CommandData[4] = byteArr[3];
            m_cmd.CommandData[5] = (byte)currency[0];
            m_cmd.CommandData[6] = (byte)currency[1];
            m_cmd.CommandData[7] = (byte)currency[2];
            m_cmd.CommandDataLength = 8;

            if (!SendCommand(log)) return;
            if (CheckGenericResponses(log))
            {
                if (m_cmd.ResponseData[1] == 0x00)
                    b = true;
                else
                    b = false;
            }
        }
        // This function gets the serial number of the device.  An optional Device parameter can be used
        // for TEBS systems to specify which device's serial number should be returned.
        // 0x00 = NV200
        // 0x01 = SMART Payout
        // 0x02 = Tamper Evident Cash Box.
        public void GetSerialNumber(byte Device, TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_GET_SERIAL_NUMBER;
            m_cmd.CommandData[1] = Device;
            m_cmd.CommandDataLength = 2;


            if (!SendCommand(log)) return;
            if (CheckGenericResponses(log) && log != null)
            {
                // Response data is big endian, so reverse bytes 1 to 4.
                Array.Reverse(m_cmd.ResponseData, 1, 4);
                //log.AppendText("Serial Number Device " + Device + ": ");
                //log.AppendText(BitConverter.ToUInt32(m_cmd.ResponseData, 1).ToString());
                //log.AppendText("\r\n");
            }
        }

        public void GetSerialNumber(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_GET_SERIAL_NUMBER;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return;
            if (CheckGenericResponses(log) && log != null)
            {
                // Response data is big endian, so reverse bytes 1 to 4.
                Array.Reverse(m_cmd.ResponseData, 1, 4);
                //log.AppendText("Serial Number ");
                //log.AppendText(": ");
                //log.AppendText(BitConverter.ToUInt32(m_cmd.ResponseData, 1).ToString());
                //log.AppendText("\r\n");
            }
        }
        public bool DoPoll(TextBox log)
        {

            //if (DispensingCounter > Cart.CartItems.Count * 7)
            //{
            //    DispensingCounter = 0;
            //    Global.NextAction = Global.ActionList.GoHome;
            //    Global.UserMessage = "Apologies! some server error was occured";
            //}
            //if (GivingChangeCounter > 50)
            //{
            //    GivingChangeCounter = 0;
            //    Global.NextAction = Global.ActionList.GoHome;
            //    Global.UserMessage = "Apologies! some server error was occured";
            //}
            //LogPollingInterval();
            byte i;
            //If note is held in escrow, send hold commands
            if (m_HoldCount > 0)
            {
                m_NoteHeld = true;
                m_HoldCount--;
                m_cmd.CommandData[0] = CCommands.SSP_CMD_HOLD;
                m_cmd.CommandDataLength = 1;
                Console.WriteLine("Note held in escrow: " + m_HoldCount + "\r\n");
                if (!SendCommand(log)) return false;
                return true;

            }
            //send poll
            m_NoteHeld = false;
            m_cmd.CommandData[0] = CCommands.SSP_CMD_POLL;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log))
            {
                //added as heart beat
                Global.NoteValidator.SSP_POLL_NV_WORKING_STATUS = false;
                return false;
            }
            else
            {
                Global.NoteValidator.SSP_POLL_NV_WORKING_STATUS = true;
            }

            // Store poll response to avoid corruption if the cmd structure is accessed whilst polling
            m_cmd.ResponseData.CopyTo(m_CurrentPollResponse, 0);
            m_CurrentPollResponseLength = m_cmd.ResponseDataLength;

            //parse poll m_CurrentPollResponse
            int noteVal = 0;
            for (i = 1; i < m_CurrentPollResponseLength; i++)
            {
                switch (m_CurrentPollResponse[i])
                {
                    // This m_CurrentPollResponse indicates that the unit was reset and this is the first time a poll
                    // has been called since the reset.
                    case CCommands.SSP_POLL_SLAVE_RESET:
                        Console.WriteLine("Unit reset\r\n");
                        Global.NoteValidator.SSP_POLL_SLAVE_RESET_STATUS = true;
                        UpdateData();
                        break;
                    // A note is currently being read by the validator sensors. The second byte of this response
                    // is zero until the note's type has been determined, it then changes to the channel of the 
                    // scanned note.
                    case CCommands.SSP_POLL_READ_NOTE:
                        if (m_CurrentPollResponse[i + 1] > 0)
                        {
                            noteVal = GetChannelValue(m_CurrentPollResponse[i + 1]);
                            Console.WriteLine("Note in escrow, amount: " + CHelpers.FormatToCurrency(noteVal) + " " + GetChannelCurrency(m_CurrentPollResponse[i + 1]) + "\r\n");
                            Global.NoteValidator.SSP_POLL_READ_NOTE_STATUS = true;
                            m_HoldCount = m_HoldNumber;

                            Global.NoteValidator.CurrentNoteValue = CHelpers.FormatToCurrency(noteVal);
                            Global.NoteValidator.CurrentNoteCurrency = GetChannelCurrency(m_CurrentPollResponse[i + 1]);

                            //Checking the storage, if storage is empty and noteValue Deposited is greater than the required amount then note is returned
                            //checking for 50 for now

                            string storage_info = GetStorageInfo();

                            //if (storage_info.Length > 0)
                            if (storage_info.Contains("Position"))
                            {
                                Global.NoteValidator.StorageStatus = true;
                                Console.WriteLine("Notes are in Storage");
                            }
                            else
                            {
                                Global.NoteValidator.StorageStatus = false;
                                Console.WriteLine("Storage is Empty");
                            }
                            //PayoutStorageNumberNV11();

                            //Checking the storage 
                        }
                        else
                            Console.WriteLine("Reading note\r\n");
                        i++;
                        break;
                    // A credit event has been detected, this is when the validator has accepted a note as legal currency.
                    case CCommands.SSP_POLL_CREDIT_NOTE:
                        noteVal = GetChannelValue(m_CurrentPollResponse[i + 1]);
                        Console.WriteLine("Credit " + CHelpers.FormatToCurrency(noteVal) + " " + GetChannelCurrency(m_CurrentPollResponse[i + 1]) + "\r\n");
                        Global.NoteValidator.SSP_POLL_CREDIT_NOTE_STATUS = true;
                        NotesAccepted++;
                        UpdateData();
                        i++;
                        break;
                    // A note is being rejected from the validator. This will carry on polling while the note is in transit.
                    case CCommands.SSP_POLL_NOTE_REJECTING:
                        Global.NoteValidator.SSP_POLL_NOTE_REJECTING_STATUS = true;
                        break;
                    // A note has been rejected from the validator. This response only appears once.
                    case CCommands.SSP_POLL_NOTE_REJECTED:
                        Console.WriteLine("Note rejected\r\n");
                        Global.NoteValidator.SSP_POLL_NOTE_REJECTED_STATUS = true;
                        Global.NoteValidator.SSP_POLL_NOTE_REJECTING_STATUS = false;
                        QueryRejection(log);
                        UpdateData();
                        break;
                    // A note is in transit to the cashbox.
                    case CCommands.SSP_POLL_NOTE_STACKING:
                        // Console.WriteLine("Stacking note\r\n");
                        Global.NoteValidator.SSP_POLL_NOTE_STACKING_STATUS = true;
                        Global.NoteValidator.SSP_POLL_NOTE_STACKED_STATUS = false;
                        break;
                    // A note has reached the cashbox.
                    case CCommands.SSP_POLL_NOTE_STACKED:
                        Console.WriteLine("Note stacked\r\n");
                        Global.NoteValidator.PROCESSING = false;
                        //new TransactionHandler().SubmitCashTransaction(Global.General.CASH_IN, "Note", 0, Convert.ToInt32(Global.NoteValidator.CurrentNoteValue.Split('.')[0]));
                        Global.NoteValidator.SSP_POLL_NOTE_STACKED_STATUS = true;
                        Global.NoteValidator.SSP_POLL_NOTE_STACKING_STATUS = false;
                        break;
                    // A safe jam has been detected. This is where the user has inserted a note and the note
                    // is jammed somewhere that the user cannot reach.
                    case CCommands.SSP_POLL_SAFE_NOTE_JAM:
                        Console.WriteLine("Safe jam\r\n");
                        Global.NoteValidator.SSP_POLL_SAFE_NOTE_JAM_STATUS = true;
                        break;
                    // An unsafe jam has been detected. This is where a user has inserted a note and the note
                    // is jammed somewhere that the user can potentially recover the note from.
                    case CCommands.SSP_POLL_UNSAFE_NOTE_JAM:
                        Console.WriteLine("Unsafe jam\r\n");
                        Global.NoteValidator.SSP_POLL_UNSAFE_NOTE_JAM_STATUS = true;
                        break;
                    // The validator is disabled, it will not execute any commands or do any actions until enabled.
                    case CCommands.SSP_POLL_DISABLED:
                        // Console.WriteLine("Unit disabled...\r\n");
                        Global.NoteValidator.SSP_POLL_DISABLED_STATUS = true;
                        //WriteToFile("Note validator get disabled. recent action was " + Global.NextAction.ToString());
                        break;
                    // A fraud attempt has been detected. The second byte indicates the channel of the note that a fraud
                    // has been attempted on.
                    case CCommands.SSP_POLL_FRAUD_ATTEMPT:
                        Console.WriteLine("Fraud attempt, note type: " + GetChannelValue(m_CurrentPollResponse[i + 1]) + "\r\n");
                        Global.NoteValidator.SSP_POLL_FRAUD_ATTEMPT_STATUS = true;
                        i++;
                        break;
                    // The stacker (cashbox) is full.
                    case CCommands.SSP_POLL_STACKER_FULL:
                        Console.WriteLine("Stacker full\r\n");
                        Global.NoteValidator.SSP_POLL_STACKER_FULL_STATUS = true;
                        break;
                    // A note was detected somewhere inside the validator on startup and was rejected from the front of the
                    // unit.
                    case CCommands.SSP_POLL_NOTE_CLEARED_FROM_FRONT:
                        Console.WriteLine(GetChannelValue(m_CurrentPollResponse[i + 1]) + " " + GetChannelCurrency(m_CurrentPollResponse[i + 1]) + " note cleared from front at reset." + "\r\n");
                        Global.NoteValidator.SSP_POLL_NOTE_CLEARED_FROM_FRONT_STATUS = true;
                        i++;
                        break;
                    // A note was detected somewhere inside the validator on startup and was cleared into the cashbox.
                    case CCommands.SSP_POLL_NOTE_CLEARED_TO_CASHBOX:
                        Console.WriteLine(GetChannelValue(m_CurrentPollResponse[i + 1]) + " " + GetChannelCurrency(m_CurrentPollResponse[i + 1]) + " note cleared to stacker at reset." + "\r\n");
                        Global.NoteValidator.SSP_POLL_NOTE_CLEARED_TO_CASHBOX_STATUS = true;
                        i++;
                        break;
                    // The cashbox has been removed from the unit. This will continue to poll until the cashbox is replaced.
                    case CCommands.SSP_POLL_CASHBOX_REMOVED:
                        Console.WriteLine("Cashbox removed\r\n");
                        Global.NoteValidator.SSP_POLL_CASHBOX_REMOVED_STATUS = true;
                        break;
                    // The cashbox has been replaced, this will only display on a poll once.
                    case CCommands.SSP_POLL_CASHBOX_REPLACED:
                        Console.WriteLine("Cashbox replaced\r\n");
                        Global.NoteValidator.SSP_POLL_CASHBOX_REPLACED_STATUS = true;
                        break;
                    // A note has been stored in the payout device to be paid out instead of going into the cashbox.
                    case CCommands.SSP_POLL_NOTE_STORED_IN_PAYOUT:
                        Console.WriteLine("Note stored\r\n");
                        Global.NoteValidator.PROCESSING = false;
                        //new TransactionHandler().SubmitCashTransaction(Global.General.CASH_IN, "Note", 0, 10);
                        Global.NoteValidator.SSP_POLL_NOTE_STORED_IN_PAYOUT_STATUS = true;
                        i += (byte)((m_CurrentPollResponse[i + 1] * 7) + 1);
                        UpdateData();
                        break;
                    // The validator is in the process of paying out a note, this will continue to poll until the note has 
                    // been fully dispensed and removed from the front of the validator by the user.
                    case CCommands.SSP_POLL_DISPENSING:
                        //Console.WriteLine("Dispensing note\r\n");
                        Global.NoteValidator.SSP_POLL_DISPENSING_STATUS = true;
                        //Global.NoteValidator.SSP_POLL_DISPENSED_STATUS = false;

                        i += (byte)((m_CurrentPollResponse[i + 1] * 7) + 1);
                        break;
                    // The note has been dispensed and removed from the bezel by the user.
                    case CCommands.SSP_POLL_DISPENSED:
                        for (int j = 0; j < m_CurrentPollResponse[i + 1]; j += 7)
                        {
                            Console.WriteLine("Dispensed " + (CHelpers.ConvertBytesToInt32(m_CurrentPollResponse, j + i + 2) / 100).ToString() +
                                " " + (char)m_CurrentPollResponse[j + i + 6] + (char)m_CurrentPollResponse[j + i + 7] +
                                (char)m_CurrentPollResponse[j + i + 8] + "\r\n");
                        }
                        Global.NoteValidator.SSP_POLL_DISPENSED_STATUS = true;
                        Global.NoteValidator.SSP_POLL_DISPENSING_STATUS = false;
                        //new TransactionHandler().SubmitCashTransaction(Global.General.CASH_OUT, "Note", 0, 10);
                        i += (byte)((m_CurrentPollResponse[i + 1] * 7) + 1);
                        NotesDispensed++;
                        UpdateData();
                        EnableValidator();
                        break;
                    // A note has been transferred from the payout storage to the cashbox
                    case CCommands.SSP_POLL_NOTE_TRANSFERED_TO_STACKER:
                        Console.WriteLine("Note stacked\r\n");
                        //Global.NoteValidator.PROCESSING = false;
                        Global.NoteValidator.SSP_POLL_NOTE_TRANSFERED_TO_STACKER_STATUS = true;
                        UpdateData();
                        EnableValidator();
                        break;
                    // This single poll response indicates that the payout device has finished emptying.
                    case CCommands.SSP_POLL_EMPTIED:
                        Console.WriteLine("Device emptied\r\n");
                        Global.NoteValidator.SSP_POLL_EMPTIED_STATUS = true;
                        UpdateData();
                        EnableValidator();
                        break;
                    // This response indicates a note is being dispensed and is resting in the bezel waiting to be removed
                    // before the validator can continue
                    case CCommands.SSP_POLL_NOTE_HELD_IN_BEZEL:
                        for (int j = 0; j < m_CurrentPollResponse[i + 1]; j += 7)
                        {
                            //Console.WriteLine((CHelpers.ConvertBytesToInt32(m_CurrentPollResponse, j + i + 2) / 100).ToString() +
                            //   " " + (char)m_CurrentPollResponse[j + i + 6] + (char)m_CurrentPollResponse[j + i + 7] +
                            //   (char)m_CurrentPollResponse[j + i + 8] + " held in bezel...\r\n");
                        }
                        Global.NoteValidator.SSP_POLL_NOTE_HELD_IN_BEZEL_STATUS = true;
                        i += (byte)((m_CurrentPollResponse[i + 1] * 7) + 1);
                        break;

                    case CCommands.SSP_POLL_SMART_EMPTYING:
                        Console.WriteLine((CHelpers.ConvertBytesToInt32(m_CurrentPollResponse, i + 2) / 100).ToString() +
                               " " + (char)m_CurrentPollResponse[i + 6] + (char)m_CurrentPollResponse[i + 7] +
                               (char)m_CurrentPollResponse[i + 8] + " emptied so far...\r\n");
                        i += 9;
                        break;

                    case CCommands.SSP_POLL_SMART_EMPTIED:
                        Console.WriteLine((CHelpers.ConvertBytesToInt32(m_CurrentPollResponse, i + 2) / 100).ToString() +
                                " " + (char)m_CurrentPollResponse[i + 6] + (char)m_CurrentPollResponse[i + 7] +
                               (char)m_CurrentPollResponse[i + 8] + " emptied in total.\r\n");
                        i += 9;
                        EnableValidator();
                        break;

                    //Default condition: do nothing
                    default:
                        break;
                }
            }
            return true;
        }

        private void LogPollingInterval()
        {
            int ms = (DateTime.Now - Global.NoteValidator.RecentPollTime).Milliseconds;
            if (ms < 250 || ms > 1000)
            {
                Console.WriteLine(ms.ToString() + "******" + Environment.NewLine);
            }
            else
            {
                Console.WriteLine(ms.ToString() + Environment.NewLine);
            }
            Global.NoteValidator.RecentPollTime = DateTime.Now;
        }

        /* Non-Command functions */

        // This function uses the set routing command to send all notes to the cashbox, it just calls the
        // ChangeNoteRoute() function for each channel.
        public void RouteAllToStack(TextBox log = null)
        {
            // all notes from channels need setting
            foreach (ChannelData d in m_UnitDataList)
            {
                ChangeNoteRoute(d.Value, d.Currency, true, log);
            }
        }

        // This function returns a formatted string of what notes are available in the NV11 storage device.
        public string GetStorageInfo()
        {
            StringBuilder sb = new StringBuilder("", 100);

            sb.Append(m_StoredCurrency);
            Console.WriteLine("Stored Note  *******************" + m_StoredCurrency);
            sb.Append("\r\n");
            for (int i = 0; i < m_NotePositionValues.Length; i++)
            {
                if (m_NotePositionValues[i] > 0)
                {
                    sb.Append("Position ");
                    sb.Append(i);
                    sb.Append(": ");
                    sb.Append(m_NotePositionValues[i] / 100);
                    sb.Append(".00\r\n");
                }

            }

            return sb.ToString();
        }

        // This function updates the internal structures to check they are in sync with the validator.
        void UpdateData()
        {
            foreach (ChannelData d in m_UnitDataList)
            {
                IsNoteRecycling(d.Value, d.Currency, ref d.Recycling);
            }
            CheckForStoredNotes();
        }

        public void ShowAllRouting(TextBox log)
        {
            UpdateData();
            string Route;
            m_StoredCurrency = "";
            foreach (ChannelData d in m_UnitDataList)
            {

                if (d.Recycling)
                {
                    Route = " storage";
                    m_StoredCurrency = GetChannelCurrency(d.Channel);
                }
                else
                {
                    Route = " cashbox";
                }

                //log.AppendText(CHelpers.FormatToCurrency(d.Value) + " " + GetChannelCurrency(d.Channel) + Route + "\r\n");
            }

        }

        // This function calls the open com port function of the SSP library.
        public bool OpenComPort(TextBox log = null)
        {
            if (log != null)
                ////log.AppendText("Opening com port\r\n");
                if (!m_eSSP.OpenSSPComPort(m_cmd))
                {
                    return false;
                }
            return true;
        }

        /* Exception and Error Handling */

        // This is used for generic response error catching, it outputs the info in a
        // meaningful way.
        private bool CheckGenericResponses(TextBox log)
        {
            if (m_cmd.ResponseData[0] == CCommands.SSP_RESPONSE_OK)
                return true;
            else
            {
                if (log != null)
                {
                    switch (m_cmd.ResponseData[0])
                    {
                        case CCommands.SSP_RESPONSE_COMMAND_CANNOT_BE_PROCESSED:
                            if (m_cmd.ResponseData[1] == 0x03)
                            {
                                Console.WriteLine("NV busy...");
                                Global.NoteValidator.PROCESSING = true;
                                //log.AppendText("Validator has responded with \"Busy\", command cannot be processed at this time\r\n");
                            }
                            else
                            {
                                //log.AppendText("Command response is CANNOT PROCESS COMMAND, error code - 0x"
                                //  + BitConverter.ToString(m_cmd.ResponseData, 1, 1) + "\r\n");
                            }
                            return false;
                        case CCommands.SSP_RESPONSE_FAIL:
                            //log.AppendText("Command response is FAIL\r\n");
                            return false;
                        case CCommands.SSP_RESPONSE_KEY_NOT_SET:
                            //log.AppendText("Command response is KEY NOT SET, renegotiate keys\r\n");
                            return false;
                        case CCommands.SSP_RESPONSE_PARAMETER_OUT_OF_RANGE:
                            //log.AppendText("Command response is PARAM OUT OF RANGE\r\n");
                            return false;
                        case CCommands.SSP_RESPONSE_SOFTWARE_ERROR:
                            //log.AppendText("Command response is SOFTWARE ERROR\r\n");
                            return false;
                        case CCommands.SSP_RESPONSE_COMMAND_NOT_KNOWN:
                            //log.AppendText("Command response is UNKNOWN\r\n");
                            return false;
                        case CCommands.SSP_RESPONSE_WRONG_NO_PARAMETERS:
                            //log.AppendText("Command response is WRONG PARAMETERS\r\n");
                            return false;
                        default:
                            return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool SendCommand(TextBox log)
        {
            // Backup data and length in case we need to retry
            byte[] backup = new byte[255];
            m_cmd.CommandData.CopyTo(backup, 0);
            byte length = m_cmd.CommandDataLength;

            // attempt to send the command
            if (m_eSSP.SSPSendCommand(m_cmd, info) == false)
            {
                m_eSSP.CloseComPort();
                //m_Comms.UpdateLog(info, true); // Update on fail
                //if (log != null) WriteToFile("Sending command failed\r\nPort status: " + m_cmd.ResponseStatus.ToString() + "\r\n");
                return false;
            }

            // update the log after every command
            //m_Comms.UpdateLog(info);
            return true;
        }
        //public void PayoutStorageNumberNV11()
        //{
        //    if (!(Cart.GetTotalAmount() >= Cart.GetTotalPurchaseAmount()) || Global.PayCard.RECHARGING_IN_PROCESS)
        //    {
        //        string storageNumber_info = GetStorageInfo();
        //        Global.NoteValidator.PayoutNoteCount = storageNumber_info.Split(new string[] { "10" }, StringSplitOptions.None).Count() - 1;
        //        Console.WriteLine(storageNumber_info);
        //        Console.WriteLine("Number of PKR 50 Notes in Payout is :  " + Global.NoteValidator.PayoutNoteCount);
        //        //if (Global.NoteValidator.TotalAmount > 150)
        //        //{
        //        //    DisableValidator();
        //        //}
        //        //else
        //        if (Global.NoteValidator.CurrentNoteValue.Contains("10.00") || Global.NoteValidator.CurrentNoteValue.Contains("20.00"))
        //        {
        //            //Console.WriteLine("PKR "+Global.NoteValidator.CurrentNoteValue+" is accepted");
        //            Global.NoteValidator.CreditedNoteValue = Global.NoteValidator.CurrentNoteValue;
        //        }
        //        else if (Global.NoteValidator.CurrentNoteValue.Contains("50.00"))
        //        {
        //            //Console.WriteLine("PKR 50.00 is Value Accepted");
        //            Global.NoteValidator.CreditedNoteValue = Global.NoteValidator.CurrentNoteValue;

        //        }
        //        else if (!Global.NoteValidator.CurrentNoteValue.Contains("50.00") && Global.NoteValidator.PayoutNoteCount == 0)
        //        {
        //            //storage is empty
        //            ReturnNote();
        //            //Console.WriteLine("Note rejected , as Payout unit is empty");
        //            Global.NoteValidator.CreditedNoteValue = "0.00";
        //        }
        //        if (Global.NoteValidator.CurrentNoteValue.Contains("100.00") && Global.NoteValidator.PayoutNoteCount >= 1)
        //        {
        //            // Console.WriteLine("PKR 100.00 is Value Accepted");
        //            Global.NoteValidator.CreditedNoteValue = Global.NoteValidator.CurrentNoteValue;

        //        }
        //        else if (Global.NoteValidator.CurrentNoteValue.Contains("100.00") && Global.NoteValidator.PayoutNoteCount == 0)
        //        {
        //            //Console.WriteLine("Note rejected , as Payout unit dosnt have suffcient Notes");

        //            Global.NoteValidator.LOW_NOTE_CHANGE = true;
        //            ReturnNote();
        //            Global.NoteValidator.CreditedNoteValue = "0.00";
        //        }
        //        if (Global.NoteValidator.CurrentNoteValue.Contains("500.00") && Global.NoteValidator.PayoutNoteCount >= 2)      //set to 2 for testing purpose only
        //        {
        //            //Console.WriteLine("PKR 500.00 is Value Accepted");
        //            Global.NoteValidator.CreditedNoteValue = Global.NoteValidator.CurrentNoteValue;

        //        }
        //        else if (Global.NoteValidator.CurrentNoteValue.Contains("500.00") && Global.NoteValidator.PayoutNoteCount < 2)     //set to 2 for testing purpose only
        //        {
        //            //Console.WriteLine("Note rejected , as Payout unit dosnt have suffcient Notes");
        //            ReturnNote();
        //            Global.NoteValidator.CreditedNoteValue = "0.00";
        //        }
        //        if (Global.NoteValidator.CurrentNoteValue.Contains("1000.00") && Global.NoteValidator.PayoutNoteCount >= 16)
        //        {
        //            //Console.WriteLine("PKR 1000.00 is Value Accepted");
        //            Global.NoteValidator.CreditedNoteValue = Global.NoteValidator.CurrentNoteValue;

        //        }
        //        else if (Global.NoteValidator.CurrentNoteValue.Contains("1000.00") && Global.NoteValidator.PayoutNoteCount < 16)
        //        {
        //            //Console.WriteLine("Note rejected , as Payout unit dosnt have suffcient Notes");
        //            ReturnNote();
        //            Global.NoteValidator.CreditedNoteValue = "0.00";
        //        }

        //        Console.WriteLine("Accepted Note Value is : " + Global.NoteValidator.CreditedNoteValue);
        //        var amount = Convert.ToDecimal(Global.NoteValidator.CreditedNoteValue);
        //        Global.NoteValidator.TotalAmount = Global.NoteValidator.TotalAmount + Convert.ToInt32(amount);
        //        //VendingMachineAmount.Instance.addAmount(Convert.ToInt32(amount));


        //    }
        //    else
        //    {
        //        Global.General.ENOUGH_AMOUNT_ENTERED = true;
        //        ReturnNote();
        //    }
        //}
        public int PayoutStorageInfo()
        {
            string storage_info = GetStorageInfo();
            //Console.WriteLine("Storage info of payout device is:  "+ storage_info);
            Global.NoteValidator.PayoutNoteCount = storage_info.Split(new string[] { "10" }, StringSplitOptions.None).Count() - 1;
            return Global.NoteValidator.PayoutNoteCount;
            //Console.WriteLine("Number of PKR 50 Notes in Payout is :  " + Global.NoteValidator.PayoutNoteCount);
        }

        //private static void WriteToFile(string data)
        //{
        //    if (!File.Exists("Log.txt"))
        //    {
        //        File.Create("Log.txt");
        //    }
        //    using (StreamWriter writer = new StreamWriter("Log.txt", true))
        //    {
        //        writer.WriteLine("-----------------------------------------------------------------------------");
        //        writer.WriteLine("Date : " + DateTime.Now.ToString());
        //        writer.WriteLine(data);
        //    }
        //}
    }
}
