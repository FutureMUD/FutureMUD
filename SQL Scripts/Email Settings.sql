-- Email Related Configuration

-- Email Templates
-- You really should edit these templates and change them to something specific to your MUD
INSERT INTO EmailTemplates (TemplateType, Content, Subject, ReturnAddress) VALUES (0, '<html>
<p>Hi, {0}!</p>
<p>Welcome to FutureMUD. You, or someone pretending to be you has registered an account on the game <a href="http://www.futuremud.org">FutureMUD</a> with this email. If this was you, and you would like to register your account, please enter the following command in game:<br><b>register {1}</b></p>
<p>We hope to see you in game soon!</p>
<p>Sincerely,<br>The FutureMUD Team</p>
<p></p>
</html>', 'Welcome to FutureMUD', 'futuremuddeveloper@gmail.com');
INSERT INTO EmailTemplates (TemplateType, Content, Subject, ReturnAddress) VALUES (1, '<html><p>Hi, {0}!<p>Your account password on FutureMUD has been reset by our staff member {1}, presumably at your request, and is now <b>{2}</b>. You can now login to your account with these details.<p>If you did not initiate this password change request, please email <a href="mailto:headadmin@futuremud.com?Subject=False%20Account%20Password%20Change">The Head Administrator</a> to commence investigative action.</p><p>Sincerely,<br>The FutureMUD Team</p></html>', 'FutureMUD Password Reset', 'futuremuddeveloper@gmail.com');
INSERT INTO EmailTemplates (TemplateType, Content, Subject, ReturnAddress) VALUES (2, '<html><p>Hi, {0}!</p><p>Congratulations, your character application to FutureMUD for the character {1} has been approved by {2}! You may now login to the game and play!</p><p>They left the following comments about your character application:</p><p>"{3}" -{2}</p><p>We hope to see you in game soon!</p><p>Sincerely,<br>The FutureMUD Team</p></html>', 'Character Application Approved', 'futuremuddeveloper@gmail.com');
INSERT INTO EmailTemplates (TemplateType, Content, Subject, ReturnAddress) VALUES (3, '<html><p>Hi, {0}!</p><p>Unfortunately, your character application to FutureMUD for the character {1} has been declined by {2}.</p><p>They left the following comments about your character application:</p><p>"{3}" -{2}</p><p>Don''t fret, this can happen sometimes for a variety of reasons and you can resubmit once you make the changes outlined above. If you believe you have been declined in error, or have questions regarding your application, please feel free to join our guest lounge and discuss it with us.</p><p>We hope to see you in game soon.</p><p>Sincerely,<br>The FutureMUD Team</p></html>', 'Character Application Declined', 'futuremuddeveloper@gmail.com');
INSERT INTO EmailTemplates (TemplateType, Content, Subject, ReturnAddress) VALUES (4, '<html><p>Hi, {0}!<p>Your account password on FutureMUD has been recently changed by someone at IP Address {1}, presumably you, and is now <b>{2}</b>. You can now login to your account with these details.<p>If you did not initiate this password change, please email <a href="mailto:headadmin@futuremud.com?Subject=False%20Account%20Password%20Change">The Head Administrator</a> to commence investigative action.</p><p>Sincerely,<br>The FutureMUD Team</p></html>', 'FutureMUD Password Change', 'futuremuddeveloper@gmail.com');
INSERT INTO EmailTemplates (TemplateType, Content, Subject, ReturnAddress) VALUES (5, '<html><p>Hi, {0}!<p>Your account email on FutureMUD has been recently changed by someone at IP Address {1}, presumably you, and is now <b>{2}</b>.<p>If you did not initiate this email change, please email <a href="mailto:headadmin@futuremud.com?Subject=False%20Account%20Email%20Change">The Head Administrator</a> to commence investigative action.</p><p>Sincerely,<br>The FutureMUD Team</p></html>', 'FutureMUD Email Changed', 'futuremuddeveloper@gmail.com');
INSERT INTO EmailTemplates (TemplateType, Content, Subject, ReturnAddress) VALUES (6, '<html><p>Hi, {0}!<p>Your (or someone claiming to be you) has initiated a request for Account Recovery for your account {0}, from IP Address {1}. A code has been generated against your account which you can use to recover it:</p><p><br><b>{2}</b><br></p><p>If you did not initiate this Account Recovery, please email <a href="mailto:headadmin@futuremud.com?Subject=False%20Account%20Email%20Change">The Head Administrator</a> to commence investigative action.</p><p>Sincerely,<br>The FutureMUD Team</p></html>', 'FutureMUD Account Recovery', 'futuremuddeveloper@gmail.com');

-- Email Settings
-- You can find most of this information from your email provider
INSERT INTO StaticConfigurations (SettingName, Definition) VALUES ('EmailServer', 
'<Definition>
  <Host>smtp.gmail.com</Host>
  <Port>587</Port>
  <EnableSSL>true</EnableSSL>
  <UseDefaultCredentials>false</UseDefaultCredentials>
  <Credentials Username="futuremuddeveloper" Password="snowraven11"/>
</Definition>'
);