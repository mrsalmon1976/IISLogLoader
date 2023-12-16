﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IISLogLoader.Test.Common
{
    public static class RandomData
    {
        private static Random _random = new Random();

        public static InternetRandomizer Internet { get; private set; } = new InternetRandomizer();

        public static NumberRandomizer Number{ get; private set; } = new NumberRandomizer();

        public static StringRandomizer String { get; private set; } = new StringRandomizer();
    }
}
