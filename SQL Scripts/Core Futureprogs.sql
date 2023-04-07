USE `dbo`;

-- Always True

insert into Futureprogs (FunctionName, FunctionComment, FunctionText, ReturnType, Category, Subcategory, Public, AcceptsAnyParameters) values ('AlwaysTrue', 'Accepts any parameters, and always returns true.', 'return true', 4, 'Core', 'Universal', b'1', b'1');

set @alwaystrueprog = LAST_INSERT_ID();

-- Always False

insert into Futureprogs (FunctionName, FunctionComment, FunctionText, ReturnType, Category, Subcategory, Public, AcceptsAnyParameters) values ('AlwaysFalse', 'Accepts any parameters, and always returns false.', 'return false', 4, 'Core', 'Universal', b'1', b'1');

set @alwaysfalseprog = LAST_INSERT_ID();

-- Always zero
insert into Futureprogs (FunctionName, FunctionComment, FunctionText, ReturnType, Category, Subcategory, Public, AcceptsAnyParameters) values ('AlwaysZero', 'Accepts any parameters, and always returns 0.', 'return 0', 2, 'Core', 'Universal', b'1', b'1');

set @alwayszeroprog = LAST_INSERT_ID();

insert into Futureprogs (FunctionName, FunctionComment, FunctionText, ReturnType, Category, Subcategory, Public, AcceptsAnyParameters) values ('AlwaysOne', 'Accepts any parameters, and always returns 1.', 'return 1', 2, 'Core', 'Universal', b'1', b'1');

set @alwaysoneprog = LAST_INSERT_ID();

insert into Futureprogs (FunctionName, FunctionComment, FunctionText, ReturnType, Category, Subcategory, Public, AcceptsAnyParameters) values ('AlwaysOneHundred', 'Accepts any parameters, and always returns 100.', 'return 100', 2, 'Core', 'Universal', b'1', b'1');

set @alwayshundredprog = LAST_INSERT_ID();

-- Is Admin

insert into Futureprogs (FunctionName, FunctionComment, FunctionText, ReturnType, Category, Subcategory, Public, AcceptsAnyParameters) values ('IsAdmin', 'Returns true if the character in question is an administrator avatar.', 'return IsAdmin(@ch)', 4, 'Core', 'Character', b'1', b'0');

set @isadminprog = LAST_INSERT_ID();

insert into Futureprogs_Parameters (FutureProgId, ParameterIndex, ParameterType, ParameterName) values (@isadminprog, 0, 8, 'ch');

-- Is Male

insert into Futureprogs (FunctionName, FunctionComment, FunctionText, ReturnType, Category, Subcategory, Public, AcceptsAnyParameters) values ('IsMale', 'Returns true if toon is male.', 'return @ch.Gender == ToGender("male")', 4, 'Core', 'Character', b'1', b'0');

set @ismaleprog = LAST_INSERT_ID();

insert into Futureprogs_Parameters (FutureProgId, ParameterIndex, ParameterType, ParameterName) values (@ismaleprog, 0, 4200, 'ch');

-- Is Female

insert into Futureprogs (FunctionName, FunctionComment, FunctionText, ReturnType, Category, Subcategory, Public, AcceptsAnyParameters) values ('IsFemale', 'Returns true if toon is female.', 'return @ch.Gender == ToGender("female")', 4, 'Core', 'Character', b'1', b'0');

set @isfemaleprog = LAST_INSERT_ID();

insert into Futureprogs_Parameters (FutureProgId, ParameterIndex, ParameterType, ParameterName) values (@isfemaleprog, 0, 4200, 'ch');