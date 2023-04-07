-- Sky Template
INSERT INTO SkyDescriptionTemplates (`Name`) VALUES ('Earth''s Sky');
set @skyid = LAST_INSERT_ID();
INSERT INTO SkyDescriptionTemplates_Values (SkyDescriptionTemplateId, LowerBound, UpperBound, Description) VALUES (@skyid, 0, 10, 'The sky is filled with light, no stars are visible.');
INSERT INTO SkyDescriptionTemplates_Values (SkyDescriptionTemplateId, LowerBound, UpperBound, Description) VALUES (@skyid, 10, 16, 'The sky is filled with light. Only the very brightest of stars are visible.');
INSERT INTO SkyDescriptionTemplates_Values (SkyDescriptionTemplateId, LowerBound, UpperBound, Description) VALUES (@skyid, 16, 17.8, 'The sky is bright and hazy with light. There are but a few stars visible.');
INSERT INTO SkyDescriptionTemplates_Values (SkyDescriptionTemplateId, LowerBound, UpperBound, Description) VALUES (@skyid, 17.8, 18.38, 'The sky is bright and hazy with light. Some few stars and constellations are visible.');
INSERT INTO SkyDescriptionTemplates_Values (SkyDescriptionTemplateId, LowerBound, UpperBound, Description) VALUES (@skyid, 18.38, 19.5, 'The sky is filled with a dim, hazy light towards the horizon. The milky way can be very faintly observed towards the zenith of the sky, and many stars are visible.');
INSERT INTO SkyDescriptionTemplates_Values (SkyDescriptionTemplateId, LowerBound, UpperBound, Description) VALUES (@skyid, 19.5, 20.49, 'The sky is relatively dark except for a faint hazy glow at the horizons. The milky way can be observed cutting across the majority of the night sky except at the horizons. The Andromeda galaxy is visible and a great many stars and constellations can be observed.');
INSERT INTO SkyDescriptionTemplates_Values (SkyDescriptionTemplateId, LowerBound, UpperBound, Description) VALUES (@skyid, 20.49, 21.51, 'The sky is brilliant, dominated by the milky way overhead and filled with many stars. The Andromeda galaxy is clearly visible and only the faintest glow on the horizon spoils the view of the cosmos.');
INSERT INTO SkyDescriptionTemplates_Values (SkyDescriptionTemplateId, LowerBound, UpperBound, Description) VALUES (@skyid, 21.51, 21.89, 'The sky is crowded with countless stars, extending to the horizon in every direction. The milky way dominates the sky, with dark, visible lanes and a noticable bulge in the centre. The Andromeda and Triangulum galaxies are both easily visible to the naked eye. Only a very faint light is visible near to the horizons.');
INSERT INTO SkyDescriptionTemplates_Values (SkyDescriptionTemplateId, LowerBound, UpperBound, Description) VALUES (@skyid, 21.89, 22, 'The sky is crowded with countless stars, extending to the horizon in every direction. The milky way dominates the sky, with dark, visible lanes and a noticable bulge in the centre. The Andromeda and Triangulum galaxies are both easily visible to the naked eye.');

-- Clock

INSERT INTO Clocks (Definition, Seconds, Minutes, Hours) VALUES ('<Clock>  <Alias>UTC</Alias>  <Description>Universal Time Clock</Description>  <ShortDisplayString>$j:$m:$s $i</ShortDisplayString>  <SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString>  <LongDisplayString>$c $I</LongDisplayString>  <SecondsPerMinute>60</SecondsPerMinute>  <MinutesPerHour>60</MinutesPerHour>  <HoursPerDay>24</HoursPerDay>  <InGameSecondsPerRealSecond>4</InGameSecondsPerRealSecond>  <SecondFixedDigits>2</SecondFixedDigits>  <MinuteFixedDigits>2</MinuteFixedDigits>  <HourFixedDigits>0</HourFixedDigits>  <NoZeroHour>true</NoZeroHour>  <NumberOfHourIntervals>2</NumberOfHourIntervals>  <HourIntervalNames>    <HourIntervalName>a.m</HourIntervalName>    <HourIntervalName>p.m</HourIntervalName>  </HourIntervalNames>  <HourIntervalLongNames>    <HourIntervalLongName>in the morning</HourIntervalLongName>    <HourIntervalLongName>in the afternoon</HourIntervalLongName>  </HourIntervalLongNames>  <CrudeTimeIntervals>    <CrudeTimeInterval text="night" Lower="-2" Upper="4"/>    <CrudeTimeInterval text="morning" Lower="4" Upper="12"/>    <CrudeTimeInterval text="afternoon" Lower="12" Upper="18"/>    <CrudeTimeInterval text="evening" Lower="18" Upper="22"/>  </CrudeTimeIntervals></Clock>', 0, 0, 0);

set @clockid = LAST_INSERT_ID();

INSERT INTO Timezones (Name, Description, OffsetMinutes, OffsetHours, ClockId) VALUES ('UTC', 'Universal Time Clock (UTC)', 0, 0, @clockid);
set @utcid = LAST_INSERT_ID();

UPDATE Clocks SET PrimaryTimezoneId = @utcid WHERE Id = @clockid;

INSERT INTO StaticConfigurations (SettingName, Definition) VALUES ('RealSecondsToInGameSeconds', '0.25');

-- Calendar

INSERT INTO Calendars (Definition, Date, FeedClockId) VALUES ('<calendar>  <alias>gregorian</alias>  <shortname>Gregorian Calendar (EN-UK)</shortname>  <fullname>The Gregorian Calendar, in English with British Date Display, circa 2012</fullname>  <description><!CDATAThe calendar created by pope Gregory to replace the Julian calendar. English edition.></description>  <shortstring>$dd/$mo/$yy</shortstring>  <longstring>$nz$ww the $dt of $mf, $yy A.D</longstring>  <wordystring>$NZ$ww on this $DT day of the month of $mf, in the $YO year of our Lord</wordystring>  <plane>earth</plane>  <feedclock>0</feedclock>  <epochyear>2010</epochyear>  <weekdayatepoch>4</weekdayatepoch>  <weekdays>    <weekday>Monday</weekday>    <weekday>Tuesday</weekday>    <weekday>Wednesday</weekday>    <weekday>Thursday</weekday>    <weekday>Friday</weekday>    <weekday>Saturday</weekday>    <weekday>Sunday</weekday>  </weekdays>  <months>    <month>      <alias>january</alias>      <shortname>jan</shortname>      <fullname>January</fullname>      <nominalorder>1</nominalorder>      <normaldays>31</normaldays>      <intercalarydays />      <specialdays />        <nonweekdays />    </month>    <month>      <alias>february</alias>      <shortname>feb</shortname>      <fullname>February</fullname>      <nominalorder>2</nominalorder>      <normaldays>28</normaldays>      <intercalarydays>        <intercalaryday>          <insertdays>1</insertdays>          <nonweekdays />          <removenonweekdays />          <specialdays/>         <removespecialdays />          <intercalaryrule>            <offset>0</offset>            <divisor>4</divisor>            <exceptions>              <intercalaryrule>                <offset>0</offset>                <divisor>100</divisor>                <exceptions>                  <intercalaryrule>                    <offset>0</offset>                    <divisor>400</divisor>                    <exceptions />                    <ands />                    <ors />                  </intercalaryrule>                </exceptions>                <ands />                <ors />              </intercalaryrule>            </exceptions>            <ands />            <ors />          </intercalaryrule>        </intercalaryday>      </intercalarydays>      <specialdays />      <nonweekdays />    </month>    <month>      <alias>march</alias>      <shortname>mar</shortname>      <fullname>March</fullname>      <nominalorder>3</nominalorder>      <normaldays>31</normaldays>      <intercalarydays />      <specialdays />      <nonweekdays />    </month>    <month>      <alias>april</alias>      <shortname>apr</shortname>      <fullname>April</fullname>      <nominalorder>4</nominalorder>      <normaldays>30</normaldays>      <intercalarydays />      <specialdays />      <nonweekdays />    </month>    <month>      <alias>may</alias>      <shortname>may</shortname>      <fullname>May</fullname>      <nominalorder>5</nominalorder>      <normaldays>31</normaldays>      <intercalarydays />      <specialdays />      <nonweekdays />    </month>    <month>      <alias>june</alias>      <shortname>jun</shortname>      <fullname>June</fullname>      <nominalorder>6</nominalorder>      <normaldays>30</normaldays>      <intercalarydays />      <specialdays />      <nonweekdays />    </month>    <month>      <alias>july</alias>      <shortname>jul</shortname>      <fullname>July</fullname>      <nominalorder>7</nominalorder>      <normaldays>31</normaldays>      <intercalarydays />      <specialdays />      <nonweekdays />    </month>    <month>      <alias>august</alias>      <shortname>aug</shortname>      <fullname>August</fullname>      <nominalorder>8</nominalorder>      <normaldays>31</normaldays>      <intercalarydays />      <specialdays />      <nonweekdays />    </month>    <month>      <alias>september</alias>      <shortname>sep</shortname>      <fullname>September</fullname>      <nominalorder>9</nominalorder>      <normaldays>30</normaldays>      <intercalarydays />      <specialdays />      <nonweekdays />    </month>    <month>      <alias>october</alias>      <shortname>oct</shortname>      <fullname>October</fullname>      <nominalorder>10</nominalorder>      <normaldays>31</normaldays>      <intercalarydays />      <specialdays />      <nonweekdays />    </month>    <month>      <alias>november</alias>      <shortname>nov</shortname>      <fullname>November</fullname>      <nominalorder>11</nominalorder>      <normaldays>30</normaldays>      <intercalarydays />      <specialdays />      <nonweekdays />    </month>    <month>      <alias>december</alias>      <shortname>dec</shortname>      <fullname>December</fullname>      <nominalorder>12</nominalorder>      <normaldays>31</normaldays>      <intercalarydays />      <specialdays>        <specialday day="25" short="Christmas" long="Christmas Day" />        <specialday day="26" short="Boxing Day" long="Boxing Day" />        <specialday day="31" short="New Years Eve" long="New Years Eve" />      </specialdays>      <nonweekdays />    </month>  </months>  <intercalarymonths /></calendar>', '1-january-2015', @clockid);

set @calendarid = LAST_INSERT_ID();

-- Celestial

INSERT INTO Celestials (Definition, Minutes, FeedClockId, CelestialYear, LastYearBump) VALUES ('<sun>
  <Name>The Sun</Name>
  <Configuration MinutesPerDay="1440" MinutesPerYear="525949" MinutesPerYearFraction="0.02" OrbitalDaysPerYear="365.242374" YearsBetweenFractionBumps="17"/>
  <Illumination PeakIllumination="98000" AlphaScatteringConstant="0.05" BetaScatteringConstant="0.035" PlanetaryRadius="6378" AtmosphericDensityScalingFactor="6.35"/>
  <Orbital OrbitalEccentricity="0.016713" OrbitalInclination="0.40927970959267024" OrbitalRotationPerDay="6.30063859969952" AltitudeOfSolarDisc="0.014486" DayNumberOfVernalEquinox="79.92" DayNumberStaticOffsetAxial="-80" DayNumberStaticOffsetElliptical="-2" />
  <Triggers>
    <Trigger Angle="-0.015184364492350668" Direction="Ascending" Echo="The edge of the sun rises over the horizon as dawn breaks." />
    <Trigger Angle="-0.015184364492350668" Direction="Descending" Echo="The sun says its goodbyes for the day and sets on the horizon" />
    <Trigger Angle="-0.20943951023931953" Direction="Ascending" Echo="The first faint traces of light begin to dim the eastern sky as dawn approaches." />
    <Trigger Angle="-0.20943951023931953" Direction="Descending" Echo="The last traces of light leave the western sky, and the night begins." />
    <Trigger Angle="-0.10471975511965977" Direction="Ascending" Echo="The eastern sky begins to come alive with colour and light as dawn approaches." />
    <Trigger Angle="-0.10471975511965977" Direction="Descending" Echo="The glow in the western sky, the last remenants of the day that was, fade away to a dim memory, heralding the evening." />
    <Trigger Angle="0.052359877559829883" Direction="Descending" Echo="Shadows lengthen and the western sky turns shades of orange and pink as the sun dips low to the horizon." />
  </Triggers>
  <ElevationDescriptions>
    <Description Text="is gone from the sky, and it is night." Lower="-1.5707963267948966192313216916398" Upper="-0.20944"/>
    <Description Text="is only visible as a faint, dim glow just beneath the {0} horizon." Lower="-0.20944" Upper="-0.10472"/>
    <Description Text="is only visible as a warm glow just beneath the {0} horizon." Lower="-0.10472" Upper="-0.0152"/>
    <Description Text="is partially visible above the {0} horizon." Lower="-0.0152" Upper="-0.00595"/>
    <Description Text="is very low in the {0} sky, colour awash with oranges and pinks." Lower="-0.00595" Upper="0.1047"/>
    <Description Text="is low in the {0} sky, casting long, dark shadows over the land." Lower="0.1047" Upper="0.21"/>
    <Description Text="is in the {0} sky, at a middle range of elevation." Lower="0.21" Upper="0.42"/>
    <Description Text="is high in the {0} sky, casting its rays far and wide." Lower="0.42" Upper="1.20943951023931953"/>
    <Description Text="is very nearly directly overhead." Lower="1.20943951023931953" Upper="1.35"/>
    <Description Text="is directly overhead, banishing shadows from the land." Lower="1.35" Upper="1.5707963267948966192313216916398"/>
  </ElevationDescriptions>
  <AzimuthDescriptions>
    <Description Text="northern" Lower="-0.19634954084936647692528676655901" Upper="0.19634954084936647692528676655901"/>
    <Description Text="north-northeastern" Lower="0.19634954084936647692528676655901" Upper="0.589048622548086"/>
    <Description Text="northeastern" Lower="0.589048622548086" Upper="0.98174770424681"/>
    <Description Text="east-northeastern" Lower="0.98174770424681" Upper="1.37444678594553"/>
    <Description Text="eastern" Lower="1.37444678594553" Upper="1.76714586764426"/>
    <Description Text="east-southeastern" Lower="1.76714586764426" Upper="2.15984494934298"/>
    <Description Text="southeastern" Lower="2.15984494934298" Upper="2.55254403104171"/>
    <Description Text="south-southeastern" Lower="2.55254403104171" Upper="2.94524311274043"/>
    <Description Text="southern" Lower="2.94524311274043" Upper="3.33794219443916"/>
    <Description Text="south-southwestern" Lower="3.33794219443916" Upper="3.73064127613788"/>
    <Description Text="southwestern" Lower="3.73064127613788" Upper="4.1233403578366"/>
    <Description Text="west-southwestern" Lower="4.1233403578366" Upper="4.51603943953533"/>
    <Description Text="western" Lower="4.51603943953533" Upper="4.90873852123405"/>
    <Description Text="west-northwestern" Lower="4.90873852123405" Upper="5.30143760293278"/>
    <Description Text="northwestern" Lower="5.30143760293278" Upper="5.6941366846315"/>
    <Description Text="north-northwestern" Lower="5.6941366846315" Upper="6.08683576633022"/>
  </AzimuthDescriptions>
</sun>', 0, @clockid, 2015, 2012);

set @celestialid = LAST_INSERT_ID();

-- Light Model

INSERT INTO StaticConfigurations (SettingName, Definition) VALUES ('LightModel', '<Definition>
  <SightDifficulties>
    <SightDifficulty Difficulty="7" Lower="0" Upper="0.0001"/>
    <SightDifficulty Difficulty="6" Lower="0.0001" Upper="0.108"/>
    <SightDifficulty Difficulty="5" Lower="0.108" Upper="10"/>
    <SightDifficulty Difficulty="4" Lower="10" Upper="50"/>
    <SightDifficulty Difficulty="3" Lower="50" Upper="100"/>
    <SightDifficulty Difficulty="2" Lower="100" Upper="250"/>
    <SightDifficulty Difficulty="1" Lower="250" Upper="1000"/>
    <SightDifficulty Difficulty="0" Lower="1000" Upper="1000000"/>
  </SightDifficulties>
  <Descriptions>
    <Description Lower="0" Upper="0.0001">
      <![CDATA[Pitch black]]>
    </Description>
    <Description Lower="0.0001" Upper="0.0011">
      <![CDATA[Almost completely dark]]>
    </Description>
    <Description Lower="0.0011" Upper="1">
      <![CDATA[Extremely dark]]>
    </Description>
    <Description Lower="1" Upper="8">
      <![CDATA[Very Dark]]>
    </Description>
    <Description Lower="8" Upper="50">
      <![CDATA[Dark]]>
    </Description>
    <Description Lower="50" Upper="100">
      <![CDATA[Dim]]>
    </Description>
    <Description Lower="100" Upper="250">
      <![CDATA[Soft]]>
    </Description>
    <Description Lower="250" Upper="750">
      <![CDATA[Normal]]>
    </Description>
    <Description Lower="750" Upper="5000">
      <![CDATA[Bright]]>
    </Description>
    <Description Lower="5000" Upper="100000">
      <![CDATA[Very bright]]>
    </Description>
    <Description Lower="100000" Upper="1000000">
      <![CDATA[Extremely bright]]>
    </Description>
  </Descriptions>
</Definition>');

-- Earth Shard

INSERT INTO Shards (`Name`, MinimumTerrestrialLux, SkyDescriptionTemplateId) VALUES ('Earth', 0.0001, @skyid);

set @shardid = LAST_INSERT_ID();

INSERT INTO Shards_Calendars (ShardId, CalendarId) VALUES (@shardid, @calendarid);

INSERT INTO Shards_Celestials (ShardId, CelestialId) VALUES (@shardid, @celestialid);

INSERT INTO Shards_Clocks (ShardId, ClockId) VALUES (@shardid, @clockid);