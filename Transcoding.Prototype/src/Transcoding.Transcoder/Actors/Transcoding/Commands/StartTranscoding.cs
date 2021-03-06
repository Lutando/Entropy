﻿using System;
using Transcoding.Transcoder.Model;
using Transcoding.Transcoder.Options;

namespace Transcoding.Transcoder.Actors.Transcoding.Commands
{
    public class StartTranscoding
    {
        public readonly string _ffmpegPath;
        public Guid TranscodingId { get; }
        public MediaFile Input { get; }
        public MediaFile Output { get; }
        public ConversionOptions ConversionOptions { get; }
        public StartTranscoding(
            Guid transcodingId,
            MediaFile input,
            MediaFile output,
            ConversionOptions options,
            string ffmpegPath)
        {
            TranscodingId = transcodingId;
            Input = input;
            Output = output;
            ConversionOptions = options;
            _ffmpegPath = ffmpegPath;
        }
    }
    
    public class StartAnalysis
    {
        public readonly string _ffmpegPath;
        public Guid TranscodingId { get; }
        public MediaFile Input { get; }
        public MediaFile Output { get; }
        public ConversionOptions ConversionOptions { get; }
        public StartAnalysis(
            Guid transcodingId,
            MediaFile input,
            MediaFile output,
            ConversionOptions options,
            string ffmpegPath)
        {
            TranscodingId = transcodingId;
            Input = input;
            Output = output;
            ConversionOptions = options;
            _ffmpegPath = ffmpegPath;
        }
    }
}