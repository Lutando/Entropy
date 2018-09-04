﻿using System;
using System.Globalization;
using System.Text;
using Transcoding.Transcoder.Model;
using Transcoding.Transcoder.Options;
using Transcoding.Transcoder.Util;

namespace Transcoding.Transcoder
{
    internal class CommandBuilder
    {
        internal static string Serialize(EngineParameters engineParameters)
        {
            switch (engineParameters.Task)
            {
                case FFmpegTask.Convert:
                    return Convert(engineParameters.InputFile, engineParameters.OutputFile, engineParameters.ConversionOptions);

                case FFmpegTask.GetMetaData:
                    return GetMetadata(engineParameters.InputFile);

                case FFmpegTask.GetThumbnail:
                    return GetThumbnail(engineParameters.InputFile, engineParameters.OutputFile, engineParameters.ConversionOptions);
                case FFmpegTask.GetWaveform:
                    return GetWaveform(engineParameters.InputFile, engineParameters.OutputFile, engineParameters.ConversionOptions);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetMetadata(MediaFile inputFile)
        {
            return string.Format("-i  \"{0}\" ", inputFile.Filename);
        }

        private static string GetThumbnail(MediaFile inputFile, MediaFile outputFile, ConversionOptions conversionOptions)
        {
            var commandBuilder = new StringBuilder();

            commandBuilder.AppendFormat(CultureInfo.InvariantCulture, " -ss {0} ", conversionOptions.Seek.GetValueOrDefault(TimeSpan.FromSeconds(1)).TotalSeconds);

            commandBuilder.AppendFormat(" -i \"{0}\" ", inputFile.Filename);
            commandBuilder.AppendFormat(" -vframes {0} ", 1);

            return commandBuilder.AppendFormat(" \"{0}\" ", outputFile.Filename).ToString();
        }

        private static string GetWaveform(MediaFile inputFile, MediaFile outputFile, ConversionOptions conversionOptions)
        {
            var commandBuilder = new StringBuilder();

            //var outputFilename = $"{Guid.NewGuid()}.png";

            commandBuilder.AppendFormat(" -i \"{0}\" ", inputFile.Filename);

            if (conversionOptions?.CustomHeight == null || conversionOptions?.CustomWidth == null)
            {
                commandBuilder.AppendFormat(
                    " -filter_complex \"aformat=channel_layouts=mono,showwavespic=s=4000x1000\" -frames:v 1 \"{0}\" ",
                    outputFile.Filename);
            }
            else
            {
                commandBuilder.AppendFormat(
                    " -filter_complex \"aformat=channel_layouts=mono,showwavespic=s={0}x{1}\" -frames:v 1 \"{2}\" ",
                    conversionOptions.CustomWidth,conversionOptions.CustomHeight,outputFile.Filename);
            }

            return commandBuilder.ToString();
        }

        private static string Convert(MediaFile inputFile, MediaFile outputFile, ConversionOptions conversionOptions)
        {
            var commandBuilder = new StringBuilder();

            // Default conversion
            if (conversionOptions == null)
                return commandBuilder.AppendFormat(" -i \"{0}\"  \"{1}\" ", inputFile.Filename, outputFile.Filename).ToString();

            // Media seek position
            if (conversionOptions.Seek != null)
                commandBuilder.AppendFormat(CultureInfo.InvariantCulture, " -ss {0} ", conversionOptions.Seek.Value.TotalSeconds);

            commandBuilder.AppendFormat(" -i \"{0}\" ", inputFile.Filename);

            // Physical media conversion (DVD etc)
            if (conversionOptions.Target != Target.Default)
            {
                commandBuilder.Append(" -target ");
                if (conversionOptions.TargetStandard != TargetStandard.Default)
                {
                    commandBuilder.AppendFormat(" {0}-{1} \"{2}\" ", conversionOptions.TargetStandard.ToLower(), conversionOptions.Target.ToLower(), outputFile.Filename);

                    return commandBuilder.ToString();
                }
                commandBuilder.AppendFormat("{0} \"{1}\" ", conversionOptions.Target.ToLower(), outputFile.Filename);

                return commandBuilder.ToString();
            }

            // Audio bit rate
            if (conversionOptions.AudioBitRate != null)
                commandBuilder.AppendFormat(" -ab {0}k", conversionOptions.AudioBitRate);

            // Audio sample rate
            if (conversionOptions.AudioSampleRate != AudioSampleRate.Default)
                commandBuilder.AppendFormat(" -ar {0} ", conversionOptions.AudioSampleRate.Remove("Hz"));

            // Maximum video duration
            if (conversionOptions.MaxVideoDuration != null)
                commandBuilder.AppendFormat(" -t {0} ", conversionOptions.MaxVideoDuration);

            // Video bit rate
            if (conversionOptions.VideoBitRate != null)
                commandBuilder.AppendFormat(" -b {0}k ", conversionOptions.VideoBitRate);

            // Video frame rate
            if (conversionOptions.VideoFps != null)
                commandBuilder.AppendFormat(" -r {0} ", conversionOptions.VideoFps);

            // Video size / resolution
            if (conversionOptions.VideoSize == VideoSize.Custom)
            {
                commandBuilder.AppendFormat(" -vf \"scale={0}:{1}\" ", conversionOptions.CustomWidth ?? -2, conversionOptions.CustomHeight ?? -2);
            }
            else if (conversionOptions.VideoSize != VideoSize.Default)
            {
                string size = conversionOptions.VideoSize.ToLower();
                if (size.StartsWith("_")) size = size.Replace("_", "");
                if (size.Contains("_")) size = size.Replace("_", "-");

                commandBuilder.AppendFormat(" -s {0} ", size);
            }

            // Video aspect ratio
            if (conversionOptions.VideoAspectRatio != VideoAspectRatio.Default)
            {
                string ratio = conversionOptions.VideoAspectRatio.ToString();
                ratio = ratio.Substring(1);
                ratio = ratio.Replace("_", ":");

                commandBuilder.AppendFormat(" -aspect {0} ", ratio);
            }

            // Video cropping
            if (conversionOptions.SourceCrop != null)
            {
                var crop = conversionOptions.SourceCrop;
                commandBuilder.AppendFormat(" -filter:v \"crop={0}:{1}:{2}:{3}\" ", crop.Width, crop.Height, crop.X, crop.Y);
            }

            if (conversionOptions.BaselineProfile)
            {
                commandBuilder.Append(" -profile:v baseline ");
            }

            return commandBuilder.AppendFormat(" \"{0}\" ", outputFile.Filename).ToString();
        }
    }
}