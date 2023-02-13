
namespace ICT
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Drawing;
    using System.Resources;
    using System.Security.AccessControl;
    using Utils.Common.Logging;
    using ATAS.Indicators;
    using ATAS.Indicators.Drawing;
    using ATAS.Indicators.Technical.Properties;
    using OFT.Attributes;
    using OFT.Rendering.Settings;

    using Brushes = System.Drawing.Brushes;
    using Color = System.Drawing.Color;
    using Pen = System.Drawing.Pen;

    [DisplayName("ICT checker")]
    [HelpLinkAttribute("https://github.com/rgutmen")]


    public class ICT : Indicator
    {
        #region Properties

        [Display(ResourceType = typeof(Resources), Name = "Height", GroupName = "Settings", Order = 5)]
        public decimal Distance
        {
            get => _distanceCandle;
            set
            {
                if (value < 0)
                    return;
                _distanceCandle = value;
                RecalculateValues();
            }
        }


        [Display(ResourceType = typeof(Resources), Name = "Inside color Buy", GroupName = "Settings", Order = 10)]
        public Color InBuyColor
        {
            get => _inColorBuy;
            set
            {
                _inColorBuy = value;
                RecalculateValues();
            }
        }

        [Display(ResourceType = typeof(Resources), Name = "Border color Buy", GroupName = "Settings", Order = 15)]
        public Color BorderBuyColor
        {
            get => _borColorBuy;
            set
            {
                _borColorBuy = value;
                RecalculateValues();
            }
        }

        [Display(ResourceType = typeof(Resources), Name = "BorderWidth", GroupName = "Settings", Order = 20)]
        public int BorderWidthBuy
        {
            get => _borderWidthBuy;
            set
            {
                _borderWidthBuy = Math.Max(1, value);
                RecalculateValues();
            }
        }

        [Display(ResourceType = typeof(Resources), Name = "Inside color Sell", GroupName = "Settings", Order = 25)]
        public Color InSellColor
        {
            get => _inColorSell;
            set
            {
                _inColorSell = value;
                RecalculateValues();
            }
        }

        [Display(ResourceType = typeof(Resources), Name = "Border color Sell", GroupName = "Settings", Order = 30)]
        public Color BorderSellColor
        {
            get => _borColorSell;
            set
            {
                _borColorSell = value;
                RecalculateValues();
            }
        }

        [Display(ResourceType = typeof(Resources), Name = "BorderWidth", GroupName = "Settings", Order = 35)]
        public int BorderWidthSell
        {
            get => _borderWidthSell;
            set
            {
                _borderWidthSell = Math.Max(1, value);
                RecalculateValues();
            }
        }
        #endregion

        #region ctor

        public ICT() : base(true)
        {


            DenyToChangePanel = true;

        }

        #endregion

        #region Protected methods

        protected override void OnInitialize()
        {
            this.LogInfo("Printing test");
        }
        protected override void OnCalculate(int bar, decimal value)
        {

            if (bar <= 5)
            {
                _calculated = false;

                return;
            }

            else
            {
                //this.LogInfo($"bar number: {bar}");
                var currentCandle = GetCandle(bar - 1);
                var prevCandle = GetCandle(bar - 2);
                var prevX2Candle = GetCandle(bar - 3);
                
                
                // If downtrend... (Three consecutive candles)                
                if (currentCandle.Close < currentCandle.Open && prevCandle.Close < prevCandle.Open && prevX2Candle.Close < prevX2Candle.Open)
                {
                    _candleHeight = (prevCandle.Open - prevCandle.Close) / InstrumentInfo.TickSize;
            
                    if (_distanceCandle == 0 || _candleHeight >= _distanceCandle)
                    {
                        var brush = new SolidBrush(ConvertColor(_inColorSell));
                        var pen = new Pen(ConvertColor(_borColorSell))
                        {
                            Width = _borderWidthSell
                        };
                        _rectangle = new DrawingRectangle(bar - 3, prevX2Candle.Low,
                                                          bar - 1, currentCandle.High,
                                                          pen, brush);
                        Rectangles.Add(_rectangle);
                    }  
                }
                // If uptrend... (Three consecutive candles)
                else if (currentCandle.Close > currentCandle.Open && prevCandle.Close > prevCandle.Open && prevX2Candle.Close > prevX2Candle.Open)
                {
                    _candleHeight = (prevCandle.Close - prevCandle.Open) / InstrumentInfo.TickSize;
                    
                    if (_distanceCandle == 0 || _candleHeight >= _distanceCandle)
                    {
                        var brush = new SolidBrush(ConvertColor(_inColorBuy));
                        var pen = new Pen(ConvertColor(_borColorBuy))
                        {
                            Width = _borderWidthBuy
                        };
                        _rectangle = new DrawingRectangle(bar - 3, prevX2Candle.High,
                                                          bar - 1, currentCandle.Low,
                                                          pen, brush);
                        Rectangles.Add(_rectangle);
                    }
                }
            }
        }

        #endregion

        #region Private methods



        private System.Drawing.Color ConvertColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

       

        #endregion

        #region private vars

        private Color _inColorBuy = Color.LightGreen;
        private Color _borColorBuy = Color.DarkGreen;
        private Color _inColorSell = Color.IndianRed;
        private Color _borColorSell = Color.DarkRed;
        private decimal _distanceCandle = 5;
        private decimal _candleHeight;
        private int _borderWidthBuy = 1;
        private int _borderWidthSell = 1;
        private bool _calculated;
        private DrawingRectangle _rectangle = new(0, 0, 0, 0, Pens.Gray, Brushes.Yellow);


        #endregion
    }
}

