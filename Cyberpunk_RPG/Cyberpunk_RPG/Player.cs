﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberpunk_RPG
{
    class Player
    {
        Texture2D playerTex;
        Texture2D projectileTex;
        Texture2D reloadDisplay;
        SpriteFont font;
        public Vector2 pos;
        Vector2 mousePos;
        Vector2 projectileStart;
        Vector2 projectileSpeed;
        Vector2 dashSpeed;
        Vector2 dashDistance = new Vector2(400, 400);
        Vector2 startingPosition = Vector2.Zero;
        Vector2 endPosition = Vector2.Zero;
        int playerSpeed;
        int ammoCount;
        bool reloading;
        float reloadTimer;
        float reloadTime;
        bool jumping = false;
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        MouseState currentMouseState;
        MouseState previousMouseState;
        public List<Projectile> projectileList;

        public Player(Texture2D playerTex, Texture2D projectileTex, Vector2 pos, SpriteFont font, Texture2D reloadDisplay)
        {
            this.playerTex = playerTex;
            this.projectileTex = projectileTex;
            this.reloadDisplay = reloadDisplay;
            this.font = font;
            this.pos = pos;
            playerSpeed = 100;
            ammoCount = 8;
            reloadTimer = 1.5f;
            reloadTime = 1.5f;
            reloading = false;
            dashSpeed = new Vector2(300, 300);
            projectileSpeed = new Vector2(500, 500);
            projectileList = new List<Projectile>();
        }

        public void Update(GameTime gameTime)
        {
            projectileStart = pos;
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
            mousePos.X = currentMouseState.X;
            mousePos.Y = currentMouseState.Y;
            
            UpdateMovement(currentKeyboardState, gameTime);
            ShootProjectile(currentKeyboardState);
            Reload(currentKeyboardState, gameTime);
            foreach (Projectile p in projectileList)
            {
                p.Update(gameTime);
                if (p.Visible == false)
                {
                    projectileList.Remove(p);
                    break;
                }
            }

            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;
        }

        private void UpdateMovement(KeyboardState currentKeyboardState, GameTime gameTime)
        {
            if (jumping == false)
            {
                if (currentKeyboardState.IsKeyDown(Keys.A) == true)
                {
                    pos.X -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (currentKeyboardState.IsKeyDown(Keys.D) == true)
                {
                    pos.X += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (currentKeyboardState.IsKeyDown(Keys.S) == true)
                {
                    pos.Y += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (currentKeyboardState.IsKeyDown(Keys.W) == true)
                {
                    pos.Y -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (currentKeyboardState.IsKeyDown(Keys.Space) == true)
                {
                    startingPosition = pos;
                    endPosition = pos += GetDirection(mousePos - startingPosition) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    jumping = true;
                }
            }

            if (jumping == true)
            {
                pos += dashSpeed * GetDirection(endPosition - startingPosition) * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (Vector2.Distance(startingPosition, pos) > 200)
                {
                    jumping = false;
                    startingPosition = Vector2.Zero;
                    endPosition = Vector2.Zero;
                }
            }

        }

        private void Reload(KeyboardState currentKeyboardState, GameTime gameTime)
        {
            if (currentKeyboardState.IsKeyDown(Keys.R) == true)
            {
                reloading = true;
            }

            if (reloading == true)
            {
                reloadTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (reloadTimer <= 0)
                {
                    reloading = false;
                    ammoCount = 8;
                    reloadTimer = reloadTime;
                }
            }
        }

        private void ShootProjectile(KeyboardState currentKeyboardState)
        {
            if (currentKeyboardState.IsKeyDown(Keys.Q) == true && previousKeyboardState.IsKeyDown(Keys.Q) == false & ammoCount >= 1 & reloading == false)
            {
                ammoCount -= 1;
                createNewProjectile(GetDirection(mousePos - pos));
            }
        }

        private void createNewProjectile(Vector2 direction)
        {
            Projectile projectile = new Projectile(projectileTex, projectileStart, projectileSpeed, direction);
            projectile.distanceCheck(pos);
            projectileList.Add(projectile);
        }

        public Vector2 GetDirection(Vector2 dir)
        {
            Vector2 newDirection = dir;
            return Vector2.Normalize(newDirection);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(playerTex, pos, Color.White);

            sb.DrawString(font, ammoCount.ToString(), pos - new Vector2(0, 30), Color.Yellow);

            if (ammoCount == 0 & reloading == false)
            {
                sb.DrawString(font, "Press R to Reload", pos - new Vector2(0, 50), Color.Yellow);
            }

            if (reloading == true)
            {
                sb.Draw(reloadDisplay, new Vector2(pos.X, pos.Y - 50), new Rectangle(0, 45, reloadDisplay.Width, 44), Color.White, 0f, new Vector2(), 0.2f, SpriteEffects.None, 1);
                sb.Draw(reloadDisplay, null, new Rectangle((int)pos.X, (int)pos.Y - 50, (int)((reloadDisplay.Width * 0.2f) * ((double)reloadTimer / reloadTime)), (int)(44 * 0.2f)), new Rectangle(0, 45, reloadDisplay.Width, 44), new Vector2(), 0f, new Vector2(0.2f, 0.2f), Color.Green, SpriteEffects.None, 1);
                sb.Draw(reloadDisplay, new Vector2(pos.X, pos.Y - 50), new Rectangle(0, 0, reloadDisplay.Width, 44), Color.White, 0f, new Vector2(), 0.2f, SpriteEffects.None, 1);
            }

            foreach (Projectile p in projectileList)
            {
                p.Draw(sb);
            }
        }
    }
}
