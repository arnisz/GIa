using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection.Emit;



namespace GantnerInstruments
{


    public class eGateUtility
    {
        public enum NETWORKSCAN_INFO
        {
            /*----------------- NetworkScan DeviceInfo ID's -----------------------------*/
            DEVICE_ID = 1,
            DEVICE_IsOperational = 2,
            DEVICE_IsMaintainable = 3,
            DEVICE_Location = 4,
            DEVICE_SerialNumber = 5,
            DEVICE_Firmware = 6,
            DEVICE_TypeName = 7,
            /*----------------- NetworkScan DeviceTypeInfo ID's -------------------------*/
            DEVICE_Main = 8,
            DEVICE_Sub = 9,
            DEVICE_Function = 10,
            DEVICE_Casing = 11,
            DEVICE_MID = 12,
            DEVICE_UniqueType = 13,
            DEVICE_VendorName = 14,
            DEVICE_SeriesName = 15,
            /*----------------- NetworkScan Networkinfo ID's ----------------------------*/
            NETWORK_IPAddress_IPv4_Dynamic = 16,
            NETWORK_IPAddress_IPv6_Dynamic = 17,
            NETWORK_IPAddress_IPv4_Static = 18,
            NETWORK_DHCP_Enabled = 19,
            NETWORK_MACAddress = 20,
            NETWORK_SubnetMask = 21,
            NETWORK_GatewayAddress = 22
        };
        public enum NETWORKSCAN_TYPES
        {
            /*----------------- NetworkScan ScanTypes  ----------------------------------*/
            ScanType_IPv4_Broadcast = 0,
            ScanType_IPv4_Multicast = 1, //-> Q.gate/pac (not Q.station) need to have FW version newer than 03.2020
            ScanType_IPv6_Multicast = 2, //-> be sure to have activated IPv6 on your network adapter (only working for Q.stations)
            ScanType_All = 3
        };



        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CDDLG_IdentifyDevices_First(int useEXT,
                                                               double scanTime,
                                                               byte[] adapterInfo,
                                                               byte[] deviceInfo,
                                                               byte[] error);
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CDDLG_IdentifyDevices_Next(
                                                               byte[] adapterInfo,
                                                               byte[] deviceInfo,
                                                               byte[] error);


        /******************************************************************************/
        /***         DecodeBufferFile
         
          Decodes a binary buffer file to a destination ASCII file. If header- and data-file
          is merged together, use only one of "SourcexxxxxxFullFilename" arguments and leave
          the other empty.
          IN:
            SourceHeaderFullFilename   : source filename of header with path;
            SourceDataFullFilename     : source filename of data with path;
            DestinationFullFilename    : destination filename with path;
          OUT:
            ErrorStr                   : error text or empty if no error (reserve >1024 bytes);
          RETURN:
            General Returncodes
        */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CDDLG_DecodeBufferFile(
                                                               byte[] SourceHeaderFullFilename,
                                                               byte[] SourceDataFullFilename,
                                                               byte[] DestinationFullFilename,
                                                               byte[] ErrorStr);


        /******************************************************************************/
        /***         ConvertBufferFile
         
          Decodes a binary buffer file to a destination (conversionkind:)
           * 0 = ASCII file. 
           * 1 = FAMOS
           * 2 = MDF (MD4)
           * 3 = Matlab 
          If header- and data-file
          is merged together, use only one of "SourcexxxxxxFullFilename" arguments and leave
          the other empty.
          IN:
            SourceHeaderFullFilename   : source filename of header with path;
            SourceDataFullFilename     : source filename of data with path;
            DestinationFullFilename    : destination filename with path;
            ConversionKind             : type of conversion (Ascii or Famos)
            ConversionSettings         : additional info (null)
          OUT:
            ErrorStr                   : error text or empty if no error (reserve >1024 bytes);
          RETURN:
            General Returncodes
        */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CDDLG_ConvertBufferFile(
                                                               byte[] SourceHeaderFullFilename,
                                                               byte[] SourceDataFullFilename,
                                                               byte[] DestinationFullFilename,
                                                               int ConversionKind,
                                                               byte[] ConversionSettings,
                                                               byte[] ErrorStr);




        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CDDLG_IdentifyVisual(string mac, 
                                                        byte[] error);

        /******************************************************************************/
        /************		Start Network Scan
        /*
            New implementation of the Networkscan. Different Types of Networkscan can be performed.
            IN:
                ScanType			    : Type of networkscan: IPv4_Broadcast, IPv4_Multicast, IPv6_Multicast or all combined
            OUT:
                NumberOfScannedDevices  : Number of scanned devices. Use to iterate with "GetDeviceInfo"
            RETURN:
                General Returncodes
        */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CDDLG_NetworkScan_StartScan( int ScanType, 
                                                               ref int NumberOfScannedDevices);


        /******************************************************************************/
        /************		Get Info from Network Scan
         /*
             Read Info from scanned devices. Use "NetworkScan_StartScan" first to get the number of available devices.
             IN:
                 index					: iterator index
                 typeID					: Info ID 
             OUT:
                 info					: requested info
             RETURN:
                 General Returncodes
         */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CDDLG_NetworkScan_GetDeviceInfo(  int index, 
                                                                    int typeID,
                                                                    byte[] info);

        /******************************************************************************/
        /**************          EnableDHCP 
        /*
	        Enables or disabled DHCP on a controller

	        IN:
		        scanType					: Type of scan protocol to use (ScanType_IPv4_Multicast or ScanType_IPv6_Multicast (previous scan needed))
		        macAddress					: MAC address of the controller
		        enable						: True to enable DHCP, false to disable
	        RETURN:
		        General Returncodes
        */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CDDLG_NetworkScan_EnableDHCP(int ScanType, 
                                                               string MacAddress,
                                                               bool enable);

        /******************************************************************************/
        /***************         SetNetworkParameter
        /*
	        Sets the network parameters of a controller

	        IN:
		        scanType					: Type of scan protocol to use (ScanType_IPv4_Multicast or ScanType_IPv6_Multicast (previous scan needed))
		        macAddress					: MAC address of the controller
		        ipAddress					: New IP address of the controller
		        subnetMask					: New subnet mask of the controller
		        defaultGateway				: New default gateway of the controller
		        dnsServer					: New DNS server of the controller (optional)
	        RETURN:
		        General Returncodes
        */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CDDLG_NetworkScan_SetNetworkParameter(    int ScanType, 
                                                                            string MacAddress,
                                                                            string IpAddress,
                                                                            string SubnetMask,
                                                                            string DefaultGateway,
                                                                            string DnsServer);

        /******************************************************************************/
        /****************        Reboot
        /*
	        Reboots a controller

	        IN:
		        scanType					: Type of scan protocol to use (ScanType_IPv4_Multicast or ScanType_IPv6_Multicast (previous scan needed))
		        macAddress					: MAC address of the controller
	        RETURN:
		        General Returncodes
        */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CDDLG_NetworkScan_Reboot(int ScanType, 
                                                           string MacAddress);



        /******************************************************************************/
        /******************************************************************************/
        /*
	        Identifies a controler (blinking)

	        IN:
		        scanType					: Type of scan protocol to use (ScanType_IPv4_Multicast or ScanType_IPv4_Broadcast)
		        macAddress					: MAC address of the controller
	        RETURN:
		        General Returncodes
        */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CDDLG_NetworkScan_Identify(int ScanType, 
                                                            string MacAddress);







    }
    public class HSP
    {
        public enum RETURNSTATES
        {
            OK=0,
            ERROR=1,
            CONNECTION_ERROR=2,
            INIT_ERROR=3,
            LIMIT_ERROR=4,
            SYNC_CONF_ERROR=5,
            MULTYUSED_ERROR=6,
            INDEX_ERROR=7,
            FILE_ERROR=8,
            NOT_READY=9,
            EXLIB_MISSING=10,
            NOT_CONNECTED=11,
            NO_FILE=12,
            CORE_ERROR=13,
            POINTER_INVALID=14,
			NOT_IMPLEMENTED=15,
			INVALID_TIMESTAMP=16,
			COMPLETE=17
		};

        public enum CHANNELINFO
        {
            Name=0,
            Unit=1,
            DataDirection=2,
            Format=3,
            Type=4,
            InputAccessIndex=5,
            OutputAccessIndex=6,
            TotalAccessIndex=7,
            Precision=8,
            FieldLength=9,
            VariableType = 33,
            DaTyIndex = 34,  //Data Type Index
			InputOffset = 35 //Input byte offset
        };
        public enum DEVICEINFO
        {
            //String
            Location=10,
            Address=11,
            Type=12,
            Version=13,
            TypeCode=14,
            SerialNumber=15,
            //Integer
            SampleRate=16,
            ModuleCount=17,
            ChannelCount=18,

	        DEVICE_MID=50,
            DEVICE_BUFFERCOUNT=51,
	        DEVICE_LOGGERCOUNT=52,
            DEVICE_TSTYPE=53,
			DEVICE_DATAFRAMEWIDTH=54,
			DEVICE_ENDIANNESS=55
        };
        public enum MODULEINFO
        {
            //String
            Type = 19,
            TypeCode = 20,
            Location = 21,
            //Integer
            UARTIndex = 22,
            Address = 23,
            VariableCount = 24
        };
        public enum STORAGEINFO
        {
            STORE_FILECOUNT=25,
            STORE_SECONDS=26
        };
        public enum DATADIRECTION
        {
            Input = 0,         // = Input
            Output = 1,        // = Output
            InputOutput=2,   // = Input/output
            Empty=3,         // = Empty
            Statistic=4	     // = Statistic Channels
        };
        public enum CONNECTIONTYPE
        {
            Online=1,
            Buffer=2,
            EconLogger=3,
            Archives=4,
            Files=5,
            Diagnostic=7,
            PostProcessBuffer=8,
            Direct=7
        };
        public enum STATISTICINFOTYPE
        {
            Connected=0,
            StackSize=1,
            DecodeTime=2
        };
        public enum DIAGNOSTICTYPE
        {
            Controller=0,
            Interface=1,
            Transport=2,
            Variable=3,
            ItemCount=4
        };
        public enum STORAGETYPE
        {
            MDF=0,
            CSV=1
        };
        public enum DATATYPE
        {
            No=0,
            Bool=1,
            SINT8=2,
            USINT8=3,
            SINT16=4,
            USINT16=5,
            SINT32=6,
            USINT32=7,
            FLOAT=8,
            SET8=9,
            SET16=10,
            SET32=11,
            DOUBLE=12,
            SINT64=13,
            USINT64=14,
            SET64=15
        };
        public enum CALLBACKTYPE
        {
            Control = 0,
            Error = 1,
            Diagnostic = 2,
            DSPData = 3,
            FReady = 4,
            Debug = 5
        };
        public enum REMOTECONTROL
        {
            Start = 0,
            Stop = 1,
            End = 2
        };
        public enum FILETYPE
        {
            DIR_ALL=0,
            FLASHAPPLICATION=1,
            FLASHDATA=2,
            USBDATA=3,
            VIRTUALSTATE=4,
            VIRTUALONLINEBUFFER=5,
            VIRTUALCIRCLEBUFFER=6,
            VIRTUALARCHIVE=7,
            VIRTUALLOGGER=8,
			FILE_IDENTIFY_BY_PATH=10
        };
        public enum VARIABLEKIND
        {
            VarKindEmpty = 0,
            VarKindAnalogOutput = 1, 
            VarKindAnalogInput = 2,
            VarKindDigitalOutput = 3, 
            VarKindDigitalInput = 4,
            VarKindArithmetic = 5,
            VarKindSetpoint = 6,
            VarKindAlarm = 7,
            VarKindPIDController = 8,
            VarKindSignalConditioning = 9,
            VarKindRemote = 10,
            VarKindReference = 11,
            VarKindUnknown = 12,
            VarKindNotDefined = 13
        };
		
		/****************************************************************************************/
        /************		Initialize connection
        Initialize the Ethernet HighSpeedPort Connection to a Gantner-Instruments		
        Controller. 																	
	    																		
        IN:
            hostName		 ...  the ip address of the controller

            timeout			 ...  the connection timeout in seconds

            mode			 ...  the communication mode (see constants "Connection Types")
								  
                                  If HSP_ONLINE: this function initializes the complete 
                                  communication.

                                  If HSP_BUFFER or HSP_LOGGER: 
                                  eGateHighSpeedPort_InitBuffer is needed to select the 
                                  buffer index before data transfer.

                                  Other Communication types will only open the Port but 
                                  not initialize anything.

            sampleRate       ...  The sample rate for reading single or buffered data 
                                  from the controller.
								  
                                  HSP_ONLINE: up to 1000Hz (check System Health!)
                                  HSP_BUFFER: 2Hz default (otherwise check System Health!)

         OUT:
	
            client Instance	  ... If several tasks of the application uses the same connection,
                                  some DLL functions need the client instance 
                                  for synchronisation.

            connectionInstance .. This dll supports up to 10 connections which 
                                  work in different threads.

                                  To identify the connection, this Instance has to be stored.

							
        RETURN:	General Returncodes	
         
        */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_Init(string hostname,
                                                              int timeout,
                                                              int mode,
                                                              int sampleRate,
                                                              ref int HCLIENT,
                                                              ref int HCONNECTION);

        /****************************************************************************************/
		
		/****************************************************************************************/
        /************		Get BostProcess buffer count
         *
	        RETURN:
		        Number of available post process buffers
        */

        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 _CD_eGateHighSpeedPort_GetPostProcessBufferCount();

        /****************************************************************************************/
		
		/****************************************************************************************/
		/************		Get info about PostProcess buffer
	        Returns basic information about one post process buffer

	        IN:
		        bufferIndex			...	buffer index
		        bufferIDLen			... maximum length of destination buffer for bufferID
		        bufferNameLen		... maximum length of destination buffer for bufferName

	        OUT:
		        bufferID			... unique buffer id (needed to connect to it)
		        bufferName			... friendly buffer name

	        RETURN:
		        General return codes

	        ATTENTION:
		        GetPostProcessBufferCount has to be called before!!
		        Not thread save! This means that a call to GetPostProcessBufferCount() from another Thread
		        could change buffer enumeration while reading single buffers!
        */

        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_GetPostProcessBufferInfo(UInt32 bufferIndex,
                                                                                 byte[] bufferID,
                                                                                int bufferIDLen,
                                                                                byte[] bufferName,
                                                                                int bufferNameLen);
		/****************************************************************************************/
		
		/****************************************************************************************/
        /************		Initialize connection to PostProcess buffer
        */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_Init_PostProcessBuffer(string BufferID,
														        ref int clientInstance,
														        ref int connectionInstance);


        /****************************************************************************************/
		
		
        //////////////////////////////////////////////////////////////////////////////////////////
        /*------------- PostProcess buffer server handling -------------------------------------*/
        /*																						*/
        /*	Description:																		*/
        /*																						*/
        /*		Following functions allow creation of PostProcess buffers / data stream			*/
        /*		Depending on environmental settings, different data backends are supported   	*/
        /*																						*/
        //////////////////////////////////////////////////////////////////////////////////////////
        /**
         * @brief Create new PostProcess buffer / SystemStream
         *
         * @param sourceID			source UUID (SID) of this buffer
         * @param sourceName		name of this buffer
         * @param measurementID		measurement UUID (MID) of the actual mesurement
         * @param measurementName	name of the actual measurement
         * @param sampleRateHz		the desired sample rate for this measurement
         * @param bufferSizeByte	the maximum size of this buffer in bytes
         * @param segmentSizeByte	the size of a buffer segment (if supported)
         * @param retentionTimeSec  data retention time of this buffer (if supported)
         *
         * @param bufferHandle		the result handle
         * @param errorMsg			buffer for error message text if not successful
         * @param errorMsgLen		length of the error message buffer
         *
         * @return General return codes
         */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_PostProcessBufferServer_Create( string sourceID,
                                                                                        string sourceName,
                                                                                        double sampleRateHz,
                                                                                        UInt64 bufferSizeByte,
                                                                                        UInt64 segmentSizeByte,
                                                                                        double retentionTimeSec,
                                                                                        string dataTypeIdent,
                                                                                        ref int bufferHandle,
                                                                                        byte[] errorMsg,
                                                                                        uint errorMsgLen);
        /*--------------------------------------------------------------------------------------*/
        /**
         * @brief Create new UDBF file buffer
         *
         * @param fullFilePath		destination path on the local file system
         * @param sourceID			source UUID (SID) of this stream
         * @param sourceName		name of this stream
         * @param sampleRateHz		the desired sample rate for this measurement
         * @param maxLengthSeconds	the maximum length of a file in seconds
         * @param options			bitset for future options
         * @param bufferHandle		the result handle
         * @param errorMsg			buffer for error message text if not successful
         * @param errorMsgLen		length of the error message buffer
         *
         * @return General return codes
         *
         * @details					Similar than the previous PostProcessBuffer, but writes to UDBF files.
         *							FullFilePath is checked if a file name is set or if it is a directory.
         *							If maxLengthSeconds is not 0, a new file is created automatically when reaching this limit.
         *							Also depending on the length, a automatic file wrap is done at round times.
         *							If it is a directory, the file name will be the source name from the header.
         *							It it is a file name, date time information will be appended.
         *							In both cases, the full path will be created if not existing.
         */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_PostProcessBufferServer_CreateUDBFFileBuffer(string fullFilePath,
                                                                                                     string  sourceID,
                                                                                                     string  sourceName,
                                                                                                     double  sampleRateHz,
                                                                                                     UInt64  maxLengthSeconds,
                                                                                                     UInt16  options,
                                                                                                     ref int bufferHandle,
                                                                                                     byte[]  errorMsg,
                                                                                                     uint    errorMsgLen);
        /*--------------------------------------------------------------------------------------*/
        /**
         * @brief Add a variable definition to the PostProcess buffer / SystemStream
         *
         * @param bufferHandle		handle to the requested buffer
         * @param variableID
         * @param variableName
         * @param Unit
         * @param DataTypeCode
         * @param VariableKindCode
         * @param Precision
         * @param FieldLength
         * @param RangeMin
         * @param RangeMax
         * @param errorMsg			buffer for error message text if not successful
         * @param errorMsgLen		length of the error message buffer
         *
         * @return General return codes
         */   
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_PostProcessBufferServer_AddVariableDefinition(  int bufferHandle,
                                                                                                        string variableID,
                                                                                                        string variableName,
                                                                                                        string Unit,
                                                                                                        int DataTypeCode,
                                                                                                        int VariableKindCode,
                                                                                                        uint Precision,
                                                                                                        uint FieldLength,
                                                                                                        double RangeMin,
                                                                                                        double RangeMax,
                                                                                                        byte[] errorMsg,
                                                                                                        uint errorMsgLen);

        /*--------------------------------------------------------------------------------------*/
        /**
         * @brief Initialize Post Process Buffer
         *
         * @param bufferHandle		handle to the requested buffer
         * @param tempBufferLength	number of temporary buffer frames to be allocated internally
         * @param errorMsg			buffer for error message text if not successful
         * @param errorMsgLen		length of the error message buffer
         *
         * @return General return codes
         *
         * @details This function finalize the configuration and really creates the buffer.\n
         * 			If successful, data can be appended to the buffer.
         * 			There is also the possibility to allocate an internal frame buffer,
         * 			which geometry automatically fits to the actual buffer configuration.
         */               
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_PostProcessBufferServer_Initialize( int bufferHandle,
                                                                                            uint frameBufferLength,
                                                                                            byte[] errorMsg,
                                                                                            uint errorMsgLen);

        /*--------------------------------------------------------------------------------------*/
        /**
         * @brief Write data to internal frame buffer
         *
         * @param bufferHandle
         * @param timeStamp
         * @param frameIndex
         * @param variableIndex
         * @param valueInt
         * @param valueDouble
         * @param errorMsg
         * @param errorMsgLen
         *
         * @return General return codes
         */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_PostProcessBufferServer_WriteDoubleToFrameBuffer(   int bufferHandle,
                                                                                                            uint frameIndex,
                                                                                                            uint variableIndex,
                                                                                                            double valueDouble,
                                                                                                            byte[] errorMsg,
                                                                                                            uint errorMsgLen);

        /*--------------------------------------------------------------------------------------*/
        /**
         *
         * @param bufferHandle
         * @param frameIndex
         * @param variableIndex
         * @param valueInt
         * @param errorMsg
         * @param errorMsgLen
         * @return
         */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_PostProcessBufferServer_WriteTimestampToFrameBuffer(    int bufferHandle,
                                                                                                                uint frameIndex,
                                                                                                                UInt64 valueInt,
                                                                                                                byte[] errorMsg,
                                                                                                                uint errorMsgLen);

        /*--------------------------------------------------------------------------------------*/
        /**
         * @brief Append the internal temporary frame buffer to the post process buffer
         *
         * @param bufferHandle		handle to the requested buffer
         * @param errorMsg			buffer for error message text if not successful
         * @param errorMsgLen		length of the error message buffer
         *
         * @return General return codes
         */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_PostProcessBufferServer_AppendFrameBuffer(  int bufferHandle,
                                                                                                    byte[] errorMsg,
                                                                                                    uint errorMsgLen);
        /*--------------------------------------------------------------------------------------*/
        /**
         * @brief Append data to PostProcess buffer / SystemStream
         *
         * @param bufferHandle		handle to the requested buffer
         * @param data				pointer to raw data to be appended
         * @param dataLength		length of data to be appended
         * @param errorMsg			buffer for error message text if not successful
         * @param errorMsgLen		length of the error message buffer
         *
         * @return General return codes
         */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_PostProcessBufferServer_AppendDataBuffer(   int bufferHandle,
                                                                                                    string data,
                                                                                                    UInt64 dataLength,
                                                                                                    byte[] errorMsg,
                                                                                                    uint errorMsgLen);
        /*--------------------------------------------------------------------------------------*/
        /**
         * @brief Close post process buffer server
         *
         * @param bufferHandle		handle to the requested buffer
         * @param errorMsg			buffer for error message text if not successful
         * @param errorMsgLen		length of the error message buffer
         *
         * @return General return codes
         */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_PostProcessBufferServer_Close(  int bufferHandle,
                                                                                        byte[] errorMsg,
                                                                                        uint errorMsgLen);
        /*--------------------------------------------------------------------------------------*/
		



		/****************************************************************************************/
		/************		Read and decode file
			This function initializes any UDBF file as it was a connection.
			Common buffer functions can be used to access the data

			IN:
				fileName			...	source file name

			OUT:
				client Instance		... If several tasks of the application uses the same connection,
										some DLL functions need the client instance for synchronisation.
				connectionInstance	... handle that identifies/selects a connection

			RETURN:
				General return codes
		*/
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_DecodeFile_Select(ref int clientInstance,
																          ref int connectionInstance,
																          string fileName);
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Configure sample rate

			Modifies the sample rate at runtime.
			This sample rate only defines the int32_terval for reading data from the controller
			to the pc.
			Due to ethernet is not deterministic, this will not be an exact timing.
			It only helps to influence the rate how fast data is copied between Controller and PC.
			The exact measurement rate of the controller has to be configured with test.commander!

			IN:
				connectionInstance	...	to select the correct connection
				sampleRate			... sampleRate(Hz)

			RETURN:
				General return codes
		*/		
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_SetSampleRate(int connectionInstance, 
															          long sampleRateHz);
		/****************************************************************************************/
	
		/****************************************************************************************/
		/************		Read sample rate													

			Read the sample rate as configured at "Init" or "Configure sample rate".
				
			IN:
				connectionInstance	...	to select the correct connection

			OUT:
				sampleRate			... sampleRate(Hz)

			RETURN:
				General return codes
		*/	
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_GetSampleRate(int connectionInstance, 
															          ref long sampleRateHz);
		/****************************************************************************************/
		
		/****************************************************************************************/
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_GetRTC(int connectionInstance,
																ref uint year,
																ref uint month,
																ref uint day,
																ref uint hour,
																ref uint minute,
																ref uint second,
																ref uint millisecond);
																
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_SetRTC(int connectionInstance,
																uint year,
																uint month,
																uint day,
																uint hour,
																uint minute,
																uint second,
																uint millisecond);
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Configure Receive Timeout											

			At eGateHighSpeedPort_init(..) connection timeout and receive timeout are similar.
			This function configures the timeout for receiving data.
			The "winsock.h" function "select()" is used to generate the timeout 
			-> no blocking while timeout

			IN:
				connectionInstance	...	to select the correct connection
				timeout				... receive timeout in seconds

			RETURN:
				General return codes
		*/
		
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_SetReceiveTimeout(int connectionInstance, 
																          long timeOut);

		/************		Read Receive Timeout												

			Reads the timeout configured with "_CD_eGateHighSpeedPort_SetReceiveTimeout"

			IN:
				connectionInstance	...	to select the correct connection
				
			OUT:
				timeout				... receive timeout in seconds

			RETURN:
				General return codes
		*/
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_GetReceiveTimeout(int connectionInstance, 
																          ref long timeOut);		
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Close connection													

			Closes an opened connection and terminates its worker threads.

			IN:
				connectionInstance	...	to select the correct connection
				clientInstance		... to select the correct client

			RETURN:
				General return codes
		*/	       
       [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
       public static extern int _CD_eGateHighSpeedPort_Close(int connectionInstance,
													        int clientInstance);
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Get number of channels		

			Reads the number of channels of a specific connection and a specific data direction

			ATTENTION: Buffered connections will not show any output channels,
					   although they are configured on the device!!
					

			IN:
				connectionInstance	...	to select the correct connection
				directionID			... to select the channel direction:

										DADI_INPUT	-> Input channels
										DADI_OUTPT	-> Output channels
										DADI_INOUT	-> Input or output channels

			OUT:
				ChannelCount			Number of channels

			RETURN:
				General return codes
		*/
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int _CD_eGateHighSpeedPort_GetNumberOfChannels(int ConnectionInstance, 
															               int directionID,
                                                                           ref int ChannelCount);
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Get device info											


			Can be used to get different system info's from a initialized connection.

			IN:
				connectionInstance	...	to select the correct connection
				typeID				...	to select the requested type:

										DEVICE_LOCATION		...	reads the device location to channelInfo[]
										DEVICE_ADDRESS		...	reads the ip Address to channelInfo[]
										DEVICE_TYPE			... reads the module type to channelInfo[]
										DEVICE_VERSION		...	reads the firmware version to channelInfo[]
										DEVICE_TYPECODE		...	reads the MK-Code to channelInfo[]
										DEVICE_SERIALNR		...	reads the serial number to channelInfo[]

										DEVICE_SAMPLERATE	... reads the sample rate to info
										DEVICE_MODULECOUNT	... reads the number of slave modules to info
										DEVICE_CHANNELCOUNT	... reads the number of channels to info

			OUT:
				info				... device info as int32_teger as selected with typeID
				channelInfo			... device info as string as selected with typeID

			RETURN:
				General return codes
		*/
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_GetDeviceInfo(int ConnectionInstance,
                                                              int typeID,
                                                              int index,
                                                              ref double info,
                                                              byte[] Info);
		/****************************************************************************************/
		
		
		/****************************************************************************************/
		/************		Get channel info - string											

			Reads channel specific text based info's by an type ID, the channel Index and direction.

			Use "_CD_eGateHighSpeedPort_GetNumberOfChannels" first to get the number of channels for the 
			desired data direction.

			Then read any neccessary info to the chanels by indexing within a loop.
			The channel order is strictly conform to the system configuration.

			The same DirectionID as for "_CD_eGateHighSpeedPort_GetNumberOfChannels" has to be used!!

			IN:
				connectionInstance	...	to select the correct connection
				typeID				...	type of info
				
										CHINFO_NAME	-> Channel name
										CHINFO_UNIT	-> Unit (°C, m, kg,...)	
										CHINFO_DADI	-> Data direction (Input, Output, Empty,..)
										CHINFO_FORM	-> Data type
										CHINFO_TYPE	-> Channel Type (analog, digital,..)

				directionID			...	similar to "_CD_eGateHighSpeedPort_GetNumberOfChannels"
				channelIndex		... to access the correct channel from the list
				channelInfo			... desired string based channel info

			OUT:
				channelInfo			...	channel info as string

			RETURN:
				General return codes
		*/
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int _CD_eGateHighSpeedPort_GetChannelInfo_String(int ConnectionInstance,
																				 int typeID,
																				 int directionID,
																				 int channelIndex,
																				 byte[] channelInfo);
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Get channel info - int
		Reads channel specific text based info's by an type ID, the channel Index and direction.

			Use "_CD_eGateHighSpeedPort_GetNumberOfChannels" first to get the number of channels for the 
			desired data direction.

			Then read any neccessary info to the chanels by indexing within a loop.
			The channel order is strictly conform to the system configuration.

			The same DirectionID as for "_CD_eGateHighSpeedPort_GetNumberOfChannels" has to be used!!

			IN:
				connectionInstance	...	to select the correct connection
				typeID				...	type of info
										CHINFO_INDI		-> Input access Index
										CHINFO_INDO		-> Output access Index
										CHINFO_INDX		-> Total access index
										CHINFO_PREC		-> precision
										CHINFO_FLEN		-> field length

				directionID			...	similar to "_CD_eGateHighSpeedPort_GetNumberOfChannels"
				channelIndex		... to access the correct channel from the list
				channelInfo			... desired numeric channel info

			OUT:
				channelInfo			...	channel info as int32_teger

			RETURN:
				General return codes
		*/	
       [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
       public static extern int _CD_eGateHighSpeedPort_GetChannelInfo_Int(int ConnectionInstance,
                                                                             int typeID,
                                                                             int directionID,
                                                                             int channelIndex,
                                                                             ref int channelInfo);
		/****************************************************************************************/
		

		/****************************************************************************************/		
		/************		Read online single channel											

			Read a single double value from a specific channel on the connection, selected 
			with connectionIndex.

			All channels(analoge, digital / floating point32_t, int32_teger, boolean,..)

			IN:
				connectionInstance	...	to select the correct connection
				channelIndex		... to access the correct channel from the list
										Here, always the total index is neccessary!!
										-> Use "eGateHighSpeedPort_GetChannelInfo_Int" to to convert 
										any IN, OUT or INOUT index to the correct total index;

			OUT:
				value				... the actual value of this channel converted to double

			RETURN:
				General return codes
		*/		
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_ReadOnline_Single(int connectionInstance,
                                                                          int HWChannelIndex,
                                                                          ref double value);
		/****************************************************************************************/
		
		/****************************************************************************************/
        /************		Read online frame to double array

	        Reads a complete or part of an online frame to a double array.
	        No worker threads are needed, every call initiates TCP/IP communication.

	        "eGateHighSpeedPort_Init" has to be used first!

	        IN:

		        connectionInstance	...	to select the correct connection

		        arrayLength			... Number of elements in "valueArray"
								        If "valueArray" is smaller than "arrayLength" this will cause a segfault!!!

		        startIndex			... index of first variable to be read
								        (this will be the first value in valueArray)

		        channelCount		... Number of channels to be read starting from "startIndex"^.
								        If channelCount is larger than arrayLength, only arrayLength channels will be read.
								        Is channelCount is -1 or larger than the real number of channels, all channels will be read.
	        OUT:

		        valueArray			... Point32_ter to a double array with at least "ArrayLength" elements
								        contains double converted values.

	        RETURN: General Returncodes
        */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_ReadOnline_FrameToDoubleArray(int connectionIndex,
                                                                                      double[] valueArray,
                                                                                      int arrayLength,
										                                              int startIndex,
										                                              int channelCount);
		/****************************************************************************************/
		
		
		/****************************************************************************************/
		/************		Write online single channel	
										
			Write a single double value to a specific channel on the connection, selected 
			with connectionIndex.

			All channels(analoge, digital / floating point32_t, int32_teger, boolean,..)

			ATTENTION: All channels can be written one by one. They will be stored in the DLL output buffer
					   until "eGateHighSpeedPort_WriteOnline_ReleaseOutputData" is called for this connection.

			IN:
				connectionInstance	...	to select the correct connection
				channelIndex		... to access the correct channel from the list
										Here, always the total index is neccessary!!
										-> Use "eGateHighSpeedPort_GetChannelInfo_Int" to to convert 
										any IN, OUT or INOUT index to the correct total index;
				value				... the new value for this channel as double
										(will be converted to the correct data type on the device)

			RETURN:
				General return codes
		*/
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_WriteOnline_Single(
                                                int connectionInstance,
                                                int channelIndex,
                                                double value);
		/****************************************************************************************/
		
		/****************************************************************************************/		
		/************		Write online single channel	Immediate
												
			Write a single double value to a specific channel on the connection, selected 
			with connectionIndex immeadiately.

			All channels(analoge, digital / floating point32_t, int32_teger, boolean,..)

			IN:
				connectionInstance	...	to select the correct connection
				channelIndex		... to access the correct channel from the list
										Here, always the total index is neccessary!!
										-> Use "eGateHighSpeedPort_GetChannelInfo_int" to to convert 
										any IN, OUT or INOUT index to the correct total index;
				value				... the new value for this channel as double
										(will be converted to the correct data type on the device)

			RETURN:
				General return codes
		*/
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_WriteOnline_Single_Immediate(
                                                int connectionInstance,
                                                int channelIndex,
                                                double value);
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Release output value										

			Releases all bufered output values.
			This ensures that all channels are written simultaniously.

			IN:
				connectionInstance	...	to select the correct connection

			RETURN:
				General return codes
		*/
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_WriteOnline_ReleaseOutputData(int connectionInstance);
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Write online multiple channels										

			Not supported

			Preliminary Version for testing reasons:

			This function writes multiple sequent OUTPUT-channels simultanuously

			IN:
				connectionInstance	...	to select the correct connection

				startIndex			... index of first OUTPUT variable to write (use Output-Var Index)

				size				... size of values-array -> number of sequent OUTPUT variables to write, beginning from "startIndex"

				valuesArray			... pointer to an array of double values. 


			RETURN:
				General return codes
		 * */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_WriteOnline_Window(
                                                int connectionInstance,
                                                int startIndex,
                                                int size,
                                                double[] valuesArray);
		/****************************************************************************************/
		
		
		
		
		
		
		/****************************************************************************************/
        /************		Initialize Buffer
												    
		Initializes a buffered connection with a specified circular buffer or test.con
		dataLogger on a supported controller.

		Communication Type must be HSP_BUFFER or HSP_LOGGER.

		eGateHighSpeedPort_Init() has to be used first!!
	    																		
 	    IN:

		    connection Instance	...	to select the correct connection

		    buffer Index		... the index of a CircleBuffer or test.con Logger.
								    Check configuration if the correct buffer type is supported
								    and configured correctly!

	    RETURN:	General Returncodes	
        
        */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_InitBuffer(int HCONNECTION,
                                                                    int bufferIndex,
                                                                    int autoRun);
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************	GetTimeRange

	        Returns the first and the last available time stamp of a buffered data source.

	        IN:
		        connection Instance	...	to select the correct connection
		        clientInstance		... to select the correct client

	        Out:
		        startTimeDC			... uint64_t pointer to the start time variable
		        startTimeDC			... uint64_t pointer to the end time variable

	        RETURN:
		        General return codes

	        ATTENTION:
		        This function can only be used with PostProcessBuffer or file connections!!

        */

        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_GetTimeRange(int connectionInstance,
														                  int clientInstance,
														                  ref UInt64 startTimeDC,
                                                                          ref UInt64 endTimeDC);
		/****************************************************************************************/
				
		/****************************************************************************************/
        /************			Set BackTime													

		Is used to set a BackTime Manually for one Ethernet Request.

		Backtime: defines how many seconds of buffer data should be read at one request.

	    IN:

		    connection Instance	...	to select the correct connection

		BackTime			... <0 : complete buffer will be read
								>=0: the next request will contain the last "BackTime" seconds
									 or the complete buffer if less than "BackTime" seconds 
									 are stored

	    RETURN:	General Returncodes	

        */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_SetBackTime(int ConnectionInstance,
                                                                    double BackTime);
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************	Seek Timestamp

	        Moves the current frame pointer to a frame with the given timestamp.

	        IN:
		        connectionInstance	...	to select the correct connection
		        clientInstance		... to select the correct client
		        timestamp			... destination timestamp

	        RETURN:
		        General return codes

	        ATTENTION:
		        This function can only be used with PostProcessBuffer or file connections!!
        */

        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_SeekTimeStamp(int connectionInstance,
														                  int clientInstance,
														                  UInt64 timestamp);
		/****************************************************************************************/

		/****************************************************************************************/
		/************	Seek

			Moves the current frame pointer forward.

			IN:
				connectionInstance	...	to select the correct connection
				clientInstance		... to select the correct client
				seekFrames			... number of frames to seek forward
				fromBeginning		... set to 1 if it should seek from beginning of the current stream

			RETURN:
				General return codes

			ATTENTION:
				This function can only be used with PostProcessBuffer or file connections!!
		*/
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int _CD_eGateHighSpeedPort_Seek(int connectionInstance,
															 int clientInstance,
															 uint seekFrames, 
															 int fromBeginning);


		/****************************************************************************************/

		/****************************************************************************************/
		/************	Rewind

	        Moves the current frame pointer back.

	        IN:
		        connectionInstance	...	to select the correct connection
		        clientInstance		... to select the correct client
		        rewindFrames		... number of frames to rewind

	        RETURN:
		        General return codes

	        ATTENTION:
		        This function can only be used with PostProcessBuffer or file connections!!
        */
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_Rewind(int connectionInstance,
                                                               int clientInstance,
														       int 	rewindFrames);
		/****************************************************************************************/

		/****************************************************************************************/
		/************		Get Buffer Frames																								

			Returns the number of available (read and decoded) data frames.
			The dll contains a FIFO with a fixed length -> data has to be read out continously
			with all clients, otherwise the data transfer will be paused 
			and the circle buffer may overun!

			IN:
				connectionInstance	...	to select the correct connection
				clientInstance		... to select the correct client
			
			RETURN:
				number of data frames
		*/
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_GetBufferFrames(int ConnectionInstance,
									                                    int ClientInstance);	
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Get Buffer Frames All

			Returns the number of all data frames of the datasource.


			IN:
			connectionInstance	...	to select the correct connection
			clientInstance		... to select the correct client

			RETURN:
			number of total frames in selected datasource
		*/
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_GetBufferFrames_All(int ConnectionInstance,
									                                    int ClientInstance);			
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Load Buffer Data

			Returns the number of available (read and decoded) data frames.
			The dll contains a FIFO with a fixed length -> data has to be read out continously
			with all clients, otherwise the data transfer will be paused
			and the circle buffer may overun!

			IN:
				connectionInstance	...	to select the correct connection
				clientInstance		... to select the correct client

			Out:
				frames				... number of available data frames

			RETURN:
				General return codes
		*/				
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_LoadBufferData(int connectionInstance,
                                                                       ref int frames);
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Next Frame														

			Used to step to the next frame.
			As long as this function isn't called for all clients,
			"eGateHighSpeedPort_ReadBuffer_Single" will not point to the next frame.

			IN:
				connectionInstance	...	to select the correct connection
				clientInstance		... to select the correct client

			RETURN:
			  General return codes
		*/
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_ReadBuffer_NextFrame(int ConnectionInstance,
												                             int ClientInstance);
		/****************************************************************************************/
		/****************************************************************************************/
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_ReadBuffer_NextChannel(int ConnectionInstance,
												                               int ClientInstance);
		/****************************************************************************************/
		
		
		
		/****************************************************************************************/
		/************		Read buffered single channel - convert to double										

			Used to read the value of a specified channel from the actual frame and converts it to double.

			IN:
				connectionInstance	...	to select the correct connection
				clientInstance		... to select the correct client
				channel index		... to select the channel index (total index)
										use "eGateHighSpeedPort_GetChannelInfo_int" to convert
										channel index if necessary.

			OUT:
				value				... value converted to double

			RETURN:
				General return codes
		*/		
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_ReadBuffer_Single(int ConnectionInstance,
										                                  int ClientInstance,
									                                      int ChannelIndex,
										                                  ref double value);
		/****************************************************************************************/
		
		/****************************************************************************************/	
		/************		Read buffer to double array									

			Reads buffered data to a double array.
			No worker threads are needed, every call initiates TCP/IP communication and data decoding.

			"eGateHighSpeedPort_Init" and "eGateHighSpeedPort_InitBuffer" has to be used first!

			IN:
				connectionInstance	...	To select the correct connection
				arrayLength			... Number of elements in "valueArray"
										If "valueArray" is smaller than "arrayLength" this will cause a segfault!!!
				fillArray			... With fillArray set to "1" this call will block until "arrayLength/channelCount"
										frames are received!

			OUT:
				valueArray			... Point32_ter to a double array with at least "ArrayLength" elements
										Contains double converted values.
				receivedFrames		... Number of frames in valueArray after processing
										(frame = 1 sample over all channels)
				channelCount		... Number of channels in one frame
										(can also be read with "getNumberOfChannels")
				complete			...	Indicates if one TCP/IP request was completely decoded


			RETURN:
			General return codes
		*/
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_ReadBufferToDoubleArray(int ConnectionInstance,
                                                                                double[] valueArray,
                                                                                int arrayLength,
                                                                                int fillArray,
                                                                                ref int receivedFrames,
                                                                                ref int channelCount,
                                                                                ref int complete);
		/****************************************************************************************/

		/****************************************************************************************/
		/************		Read buffer to Byte array

			Reads buffered raw data to a byte array.
			No worker threads are needed, every call initiates TCP/IP communication and data decoding.

			"eGateHighSpeedPort_Init" and "eGateHighSpeedPort_InitBuffer" has to be used first!

			IN:
				connectionInstance	...	To select the correct connection
				bufferSize			... size of "buffer"
										If "valueArray" is smaller than "arrayLength" this will cause a segfault!!!
				fillBuffer			... With fillBuffer set to "1" this call will block until "bufferSize/framesize"
										frames are received! (frame-aligned)

			OUT:
				buffer				... Pointer to a char array with at least "bufferSize" elements
										Contains raw values!
				receivedFrames		... Number of frames in buffer after processing
										(frame = 1 sample over all channels)
				channelCount		... Number of channels in one frame
										(can also be read with "getNumberOfChannels")
				complete			...	Indicates if one TCP/IP request was completely decoded


			RETURN:
			General return codes
		*/
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int _CD_eGateHighSpeedPort_ReadBufferToByteArray(int ConnectionInstance,
																			  byte[] buffer,
																			  int bufferSize,
																			  int fillBuffer,
																			  ref int receivedFrames,
																			  ref int channelCount,
																			  ref int complete);
		/****************************************************************************************/

		/****************************************************************************************/
		/************		Set time range limit for Read buffer

			Some buffer implementations support direct reading from a specific time range without need for reading data before or after it.
			This can save bandwidth and perfomance especially for file or network buffer backends. 

			  - Depending on the parameters, it can be used to from or and up to a defined timestamp.
			  - A Timestamp value of -1 disables the parameter.
			  - If both parameters are set to -1, time limited reading is disabled.
			  - It effects all buffer reading function if no "auto" mode is configured

			Buffer connection has to be initialized first!

			IN:
				connectionInstance	...	To select the correct connection
				StartTimeDC			... Start timestamp to read from (nano seconds since 1.1.2000)
				EndTimeDC			... End timestamp to read from (nano seconds since 1.1.2000)

			RETURN:
			General return codes

			If a time limit is set, all ReadBuffer functions will return HSP_COMPLETE as return code as the end time limit was reached
		*/
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int _CD_eGateHighSpeedPort_ReadBuffer_SetTimeRange(int ConnectionInstance,
																				UInt64 StartTimeDC,
																				UInt64 EndTimeDC);

		/****************************************************************************************/

		/****************************************************************************************/
		/************		Log to UDBF-File

			TBD -> not completely implemented yet.
			used for testing reasons...

			IN:
			connectionInstance	...	to select the correct connection

			RETURN:
			1 ....  error
			0 ....	no error
		*/
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_LogToUDBF_File(int ConnectionInstance,
																		UInt64 framecount,
																		string variableIDs,
																		string fullFileName);

		/****************************************************************************************/
		
		
		
		
		
		
		//////////////////////////////////////////////////////////////////////////////////////////
		/*------------- DLL Diagnostic ---------------------------------------------------------*/
		/*																						*/
		/*	Description:																		*/
		/*																						*/
		/*		Following functions provide diagnostic actions and information's				*/
		/*																						*/
		//////////////////////////////////////////////////////////////////////////////////////////	
		/****************************************************************************************/
		/************		DLL Version/Date													
		*/		
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void _CD_eGateHighSpeedPort_IdentDLL(byte[] DLLVersion,
                                                                 byte[] DLLDate);
																 
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_ToggleDebugMode();

        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_ExplainError(int connectionInstance, 
															        byte[] errorMessage);
																	
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_GetDebugMessage(byte[] errorMessage);

        /****************************************************************************************/
        /************		Get last error message

            get the last error information in plain text if any error-returncode is thrown


            * @param connectionInstance		to select the correct connection
            * @param errorMsg				buffer for error message text if not successful
            * @param errorMsgLen			length of the error message buffer

            RETURN:
            General return codes
        */
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_GetLastErrorMessage(int ConnectionInstance,
                                                                            byte[] errorMsg,
                                                                            uint errorMsgLen);
		
		/****************************************************************************************/
		/************		Read Connection State										
																					
			If a connection is broken (e.g. ethernet disconnect or module restart) the dll 
			will try to establish the connection agin as int32_t as the connection isn't terminated.

			This function can be used to indicate the actual connection state.
			
			IN:
				connectionInstance	...	to select the correct connection

			RETURN:
				1 ....  connection open
				0 ....	connection closed
		*/
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_Connected(int connectionInstance);
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Get Diagnostic														


			Provides system diagnostic info's
			
			Diag info's are only up to date when diagLevel==DIAG_CONTROLLER was used before!
			If errorCount != 0 then other diagLevels can be checked for errors.

			IN:
				connectionInstance	...	to select the correct connection
				diagLevel			... to request the desired info
				index				... request the info from a specified index

			OUT:	
				cycleCount			... number of communication cycles for the requested item (should be)
				errorCount			... diagLevel==DIAG_CONTROLLER:
											the number of errors over the whole system.
											Use this diagLevel to refresh diag data!
										diagLevel==DIAG_int32_tERFACE:
											the number of error on a specifc int32_terface
											(int32_ternal, UART1, UART2,...)
										diagLevel==DIAG_TRANSPORT:
											the number of error on a specifc transport
											(system variables, virtual variables, Slave1, Slave2,..)
										diagLevel==DIAG_VARIABLE:
											not used
										diagLevel==DIAG_ITEMCOUNT:
											the number of items for the defined level

			RETURN:
				General return codes
		*/		
		
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_Diagnostic(int connectionInstance,
                                                                    int diagType,
                                                                    int index,
                                                                    ref int state,
                                                                    ref int errorCount);
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Remote control command												

			This function is part of the inter process communication mechanism.

			It can be used to send commands to every process that uses the DLL
		*/		
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void _CD_eGateHighSpeedPort_RemoteControl(int remoteControlID);
		/****************************************************************************************/
		
		
		
		
		
		/****************************************************************************************/
		/*------------- File transfer functions ------------------------------------------------*/
		/*																						*/
		/*	Description:																		*/
		/*																						*/
		/*		Following functions provide file transfer - and decode functions				*/
		/*																						*/
		/*		Files can only be read or deleted completely, but not written or modified.		*/
		/*		Configuration handling has to be done via FTP									*/
		/*																						*/ 
		/*		Files can either be copied to a drive or decoded online without storing to a	*/
		/*		file.																			*/
		/*		After decoding data from local files or a file stream from the controller,		*/	
		/*		the values are accessible in the same way as reading buffer values.				*/																
		/*		(eGateHighSpeedPort_ReadBuffer_NextFrame, eGateHighSpeedPort_ReadBuffer_Single)	*/
		/*																						*/
		//////////////////////////////////////////////////////////////////////////////////////////	
		/****************************************************************************************/
		/************		Number of files														

			Used to read the number of files on the device.
			A file TypeID is also neccessary to control the desired file type

			This function will also store file specific info's which can be read with
			"eGateHighSpeedPort_GetFileInfo".

			IN:
				connectionInstance	... to select the correct connection
				fileTypeID			... to define the file type on e.gate, Q.gate, Q.pac 
										(use FILE_DIR_ALL to get all Files or 
										FILE_IDENTIFY_BY_PATH to specify a sub path on e.g. Q.Station)
				filePath			... sub path on the device

			OUT:
				fileCount			... number of files regarding file type

			RETURN:
			General return codes
		*/
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_GetFileCount(int ConnectionInstance,
                                                                        int fileTypeID,
                                                                        string filePath,
                                                                        ref int fileCount);
		/****************************************************************************************/
		
		
		/****************************************************************************************/
		/************		Get file info														

			Used to read name, size and time of a specified file.

			IN:
				connectionInstance	... to select the correct connection
				fileIndex			... index to select a certain file

			OUT:
				fileName			... fileName on device
				fileNameLen			... capacity of fileName buffer
				fileIdent			... file identification, needed to access file on device
				fileIdentLen		... capacity of fileIdent buffer
				size				... size of file
				OLETime				...	days since 01.01.1900 00:00:00
										(use eGate_OLETime2TimeStruct to convert)
										ATTENTION: 
										this time is not absolutely synchronous to the timestamp!
										
			RETURN:
				General return codes
		*/
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_GetFileInfo(int ConnectionInstance,
                                                                    int fileIndex,
                                                                    byte[] fileName,
                                                                    int fileNameLen,
                                                                    byte[] fileIdent,
                                                                    int fileIdentLen,
                                                                    ref int size,
                                                                    ref double OLETime);
		/****************************************************************************************/

		/****************************************************************************************/
		/************		Copy file															

			Used to copy a file on device to a local path

			IN:
				connectionInstance	... to select the correct connection
				fileIdent			... file identification as received with "_CD_eGateHighSpeedPort_GetFileInfo()"
				savePath			... existing path + file name

			RETURN:
				General return codes
		*/
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_CopyFile(int    ConnectionInstance,
                                                                 string fileIdent,
                                                                 string savePath);
		/****************************************************************************************/
		
		/****************************************************************************************/
		/************		Delete file															

			Used to delete a file on device

			IN:
				connectionInstance	... to select the correct connection
				fileIdent			... file identification as received with "_CD_eGateHighSpeedPort_GetFileInfo()"

			RETURN:
				General return codes
		*/
        [DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_DeleteFile(int    ConnectionInstance,
                                                                   string fileIdent);
		/****************************************************************************************/
		
		
		
		/****************************************************************************************/
		/************		Helper                ***********************************************/		
		/*
		EGATEHIGHSPEEDPORT_API 
		double CALLINGCONVENTION_CD _CD_eGate_TimeStruct2OLETime(int32_t year, 
																 int32_t month, 
																 int32_t day, 
																 int32_t hour, 
																 int32_t minute,
																 int32_t second, 
																 double belowSeconds);
		EGATEHIGHSPEEDPORT_API 
		double CALLINGCONVENTION_CD _SC_eGate_TimeStruct2OLETime(int32_t year, 
																int32_t month, 
																int32_t day, 
																int32_t hour, 
																int32_t minute,
																int32_t second, 
																double belowSeconds);
		EGATEHIGHSPEEDPORT_API 
		double CALLINGCONVENTION_CD _CD_eGate_OLETime2TimeStruct(double OLETime,
																 int32_t *year, 
																 int32_t *month, 
																 int32_t *day, 
																 int32_t *hour, 
																 int32_t *minute,
																 int32_t *second, 
																 double *belowSeconds);
		EGATEHIGHSPEEDPORT_API 
		double CALLINGCONVENTION_CD _SC_eGate_OLETime2TimeStruct(double OLETime,
																int32_t *year, 
																int32_t *month, 
																int32_t *day, 
																int32_t *hour, 
																int32_t *minute,
																int32_t *second, 
																double *belowSeconds);
		*/															
																	
			
		/****************************************************************************************/
		/************		Sleep MS											

			Can be used to sleep

			IN:
				time_msec			 ... time to sleep in milli seconds

			RETURN:
				General return codes

		*/
		[DllImport("giutility.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _CD_eGateHighSpeedPort_SleepMS(int time_msec);
		/****************************************************************************************/										
																	
    }

	public class GInsTime
    {
		[DllImport("giutility.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern UInt64 ga_GetDCTimeNSFromEpochTimeMs(ref UInt64 EpochTimeMs, bool FullSeconds);

		[DllImport("giutility.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern UInt64 ga_GetDCTimeNSFromOLETime(ref Double OLETime);

		[DllImport("giutility.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern UInt64 ga_GetEpochTimeMsFromDCTime_NS(ref UInt64 AbsTime);

		[DllImport("giutility.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern Double ga_GetOleTimeFromDCTime_NS(ref UInt64 AbsTime);

		//there are more time functions available -> see GInsTime.h

	}

    public class Help
    {
        public static void ConvertZeroTerminatedByteArray2String(out string Destination, byte[] Source) // -
        {
            int i,
                    MaxIndex;

            MaxIndex = Source.GetUpperBound(0);
            Destination = "";
            i = Source.GetLowerBound(0);
            while ((Source[i] != 0) && (i <= MaxIndex))
            {
                Destination += (char)Source[i];
                i++;
            }
            Destination = Destination.Trim();
        }

        public static void ConvertString2ZeroTerminatedByteArray(byte[] Destination, string Source) // -
        {
            bool Term;
            int i,
                    a,
                    MaxIndex;

            MaxIndex = Destination.GetUpperBound(0);
            i = Destination.GetLowerBound(0);
            Destination[i] = 0;
            a = 0;
            Term = false;
            while ((i < MaxIndex) && (a < Source.Length) && !Term)
            {
                if (Source[a] != 0)
                {
                    Destination[i] = (byte)Source[a];
                    i++;
                    a++;
                }
                else
                    Term = true;
            }
            Destination[i] = 0;
        }
    }

}